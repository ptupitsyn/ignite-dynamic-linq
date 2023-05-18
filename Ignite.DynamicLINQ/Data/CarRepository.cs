namespace Ignite.DynamicLINQ.Data;

public class CarRepository
{
    private readonly IgniteService _igniteService;

    public CarRepository(IgniteService igniteService) => _igniteService = igniteService;

    public List<Car> GetCars(string? make, string? model, int? year, SearchMode? searchMode)
    {
        // TODO: Dynamic column list as well as dynamic filters?
        var cache = _igniteService.Cars;
        throw new NotImplementedException();
    }
}
