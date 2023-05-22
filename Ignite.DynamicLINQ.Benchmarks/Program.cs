using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
using Ignite.DynamicLINQ.Data;

BenchmarkRunner.Run<CarRepositoryBenchmark>();

[MemoryDiagnoser]
public class CarRepositoryBenchmark
{
    private CarRepository _repo = null!;

    [Params(SearchMode.Any, SearchMode.All)]
    public SearchMode SearchMode { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        var igniteService = new IgniteService();
        var cars = igniteService.Cars;
        cars[1] = new Car("Ford", "Mustang", 1967);
        cars[2] = new Car("Trabant", "600", 1962);

        _repo = new CarRepository(igniteService);
    }

    [Benchmark]
    public List<Car> Linq() => _repo.GetCarsLinq("Ford", "Mustang", 1967, SearchMode);

    [Benchmark]
    public List<Car> LinqDynamic() => _repo.GetCarsLinqDynamic("Ford", "Mustang", 1967, SearchMode);

    [Benchmark]
    public List<Car> Sql() => _repo.GetCarsSql("Ford", "Mustang", 1967, SearchMode);
}
