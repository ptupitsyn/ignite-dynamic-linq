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
        using var res = _client.GetAsync("cars");
        var content = await res.Result.Content.ReadAsStringAsync();

        var expectedContent = """
            [
              {
                "make": "Ford",
                "model": "Mustang",
                "year": 1967,
                "color": "Red",
                "bodyType": "Sedan",
                "engineType": "Petrol",
                "engineCc": 7000,
                "engineHp": 335,
                "price": 25000
              }
            ]
            """;

        Assert.That(content, Is.EqualTo(expectedContent));
    }
}
