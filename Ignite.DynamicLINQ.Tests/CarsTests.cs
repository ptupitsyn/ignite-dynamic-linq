using System.Net;
using System.Text.Json;
using Ignite.DynamicLINQ.Data;
using Microsoft.AspNetCore.Mvc.Testing;

namespace Ignite.DynamicLINQ.Tests;

public class CarsTests
{
    private WebApplicationFactory<Program> _application = null!;
    private HttpClient _client = null!;

    [OneTimeSetUp]
    public void OneTimeSetUp()
    {
        _application = new WebApplicationFactory<Program>();
        _client = _application.CreateClient();

        var cars = _application.Services.GetRequiredService<IgniteService>().Cars;
        cars[1] = new Car("Ford", "Mustang", 1967, "Red", "Sedan", "Petrol", 7000, 335, 25_000);
    }

    [OneTimeTearDown]
    public void OneTimeTearDown()
    {
        _client.Dispose();
        _application.Dispose();
    }

    [Test]
    public async Task TestGetCars()
    {
        var cars = await GetCars("make=Ford&model=Mustang&searchMode=All");

        Assert.AreEqual(1, cars.Count);
    }

    private async Task<List<Car>> GetCars(string query)
    {
        using var res = await _client.GetAsync("cars?" + query);
        var content = await res.Content.ReadAsStringAsync();

        Assert.AreEqual(HttpStatusCode.OK, res.StatusCode, content);

        return JsonSerializer.Deserialize<List<Car>>(content)!;
    }
}
