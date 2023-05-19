using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using System.Text;
using Apache.Ignite.Core.Cache.Query;
using Apache.Ignite.Linq;

namespace Ignite.DynamicLINQ.Data;

public class CarRepository
{
    private static readonly string[] AllColumns = { nameof(Car.Make), nameof(Car.Model), nameof(Car.Year) };

    private readonly IgniteService _igniteService;

    public CarRepository(IgniteService igniteService) => _igniteService = igniteService;

    public List<Car> GetCarsLinq(string? make, string? model, int? year, SearchMode searchMode, string[]? columns = null)
    {
        IQueryable<Car> query = _igniteService.Cars
            .AsCacheQueryable()
            .Select(x => x.Value);

        if (make != null || model != null || year != null)
        {
            query = searchMode switch
            {
                SearchMode.All => FilterAll(),
                _ => FilterAny()
            };
        }

        if (columns != null)
        {
            // Bad attempt to have a custom column list:
            // Conditions will be translated to CASEWHEN, resulting in unnecessary complexity in the generated SQL.
            query = query.Select(x => new Car(
                columns.Contains(nameof(Car.Make))? x.Make : null!,
                columns.Contains(nameof(Car.Model))? x.Model : null!,
                columns.Contains(nameof(Car.Year))? x.Year : 0));
        }

        return query.ToList();

        IQueryable<Car> FilterAll()
        {
            if (make != null)
            {
                query = query.Where(x => x.Make == make);
            }

            if (model != null)
            {
                query = query.Where(x => x.Model == model);
            }

            if (year != null)
            {
                query = query.Where(x => x.Year == year);
            }

            return query;
        }

        IQueryable<Car> FilterAny()
        {
            var parameter = Expression.Parameter(typeof(Car), "x");

            Expression? expr = null;

            if (make != null)
            {
                expr = Expression.Equal(Expression.PropertyOrField(parameter, "Make"), Expression.Constant(make));
            }

            if (model != null)
            {
                expr = expr == null
                    ? Expression.Equal(Expression.PropertyOrField(parameter, "Model"), Expression.Constant(model))
                    : Expression.OrElse(expr, Expression.Equal(Expression.PropertyOrField(parameter, "Model"), Expression.Constant(model)));
            }

            if (year != null)
            {
                expr = expr == null
                    ? Expression.Equal(Expression.PropertyOrField(parameter, "Year"), Expression.Constant(year))
                    : Expression.OrElse(expr, Expression.Equal(Expression.PropertyOrField(parameter, "Year"), Expression.Constant(year)));
            }

            var expression = Expression.Lambda<Func<Car, bool>>(expr!, parameter);

            return query.Where(expression);
        }
    }

    public List<Car> GetCarsLinqDynamic(string? make, string? model, int? year, SearchMode searchMode, string[]? columns = null)
    {
        IQueryable<Car> query = _igniteService.Cars
            .AsCacheQueryable()
            .Select(x => x.Value);

        return query.ToList();
    }

    public List<Car> GetCarsSql(string? make, string? model, int? year, SearchMode searchMode, string[]? columns = null)
    {
        var cols = (columns?.Intersect(AllColumns) ?? AllColumns).ToList();
        var sb = new StringBuilder("SELECT ")
            .Append(string.Join(", ", cols))
            .Append(" FROM Car");

        var argIdx = 0;
        var args = new List<object>();
        AppendArg(make);
        AppendArg(model);
        AppendArg(year);

        return _igniteService.Cars.Query(new SqlFieldsQuery(sb.ToString(), args.ToArray()))
            .Select(x => new Car(
                Val<string>(x, nameof(Car.Make)),
                Val<string>(x, nameof(Car.Model)),
                Val<int>(x, nameof(Car.Year))))
            .ToList();

        void AppendArg(object? value, [CallerArgumentExpression(nameof(value))] string? name = default)
        {
            if (value != null)
            {
                sb.Append(argIdx++ == 0
                    ? " WHERE "
                    : searchMode == SearchMode.All ? " AND " : " OR ")
                    .Append($"{name} = ? ");

                args.Add(value);
            }
        }

        T Val<T>(IList<object> row, string name) => cols.IndexOf(name) is var y and >= 0 ? (T)row[y] : default!;
    }
}
