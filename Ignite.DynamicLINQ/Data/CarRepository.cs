using System.Linq.Expressions;
using Apache.Ignite.Linq;

namespace Ignite.DynamicLINQ.Data;

public class CarRepository
{
    private readonly IgniteService _igniteService;

    public CarRepository(IgniteService igniteService) => _igniteService = igniteService;

    public List<Car> GetCars(string? make, string? model, int? year, SearchMode searchMode)
    {
        // TODO: Dynamic column list as well as dynamic filters?
        IQueryable<Car> query = _igniteService.Cars
            .AsCacheQueryable()
            .Select(x => x.Value);

        if (searchMode == SearchMode.All)
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
        }
        else
        {
            Expression<Func<Car, bool>> expression = x => x.Make == make || x.Model == model || x.Year == year;

            query = query.Where(expression);
        }

        return query.ToList();
    }
}
