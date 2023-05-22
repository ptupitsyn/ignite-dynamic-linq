using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
using Ignite.DynamicLINQ.Data;

BenchmarkRunner.Run<CarRepositoryBenchmark>();

/// <summary>
/// |      Method | SearchMode |       Mean |     Error |    StdDev |   Gen0 |   Gen1 | Allocated |
/// |------------ |----------- |-----------:|----------:|----------:|-------:|-------:|----------:|
/// |        Linq |        Any |  45.740 us | 0.2229 us | 0.1976 us | 2.3193 | 0.1221 |  28.46 KB |
/// | LinqDynamic |        Any |  81.645 us | 0.4482 us | 0.3500 us | 4.8828 | 0.2441 |  61.12 KB |
/// |         Sql |        Any |   7.035 us | 0.0492 us | 0.0460 us | 0.1984 | 0.0916 |   2.45 KB |
/// |        Linq |        All | 337.704 us | 2.6861 us | 2.3812 us | 3.9063 | 2.9297 |  53.56 KB |
/// | LinqDynamic |        All |  80.868 us | 0.3346 us | 0.2794 us | 4.8828 | 0.2441 |  61.13 KB |
/// |         Sql |        All |   6.969 us | 0.0398 us | 0.0353 us | 0.1984 | 0.0916 |   2.46 KB |
/// </summary>
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
