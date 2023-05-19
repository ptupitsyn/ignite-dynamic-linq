using System.Linq.Expressions;
using Apache.Ignite.Linq;

namespace Ignite.DynamicLINQ.Data;

public class CarRepository
{
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
}
