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

        Assert.That(content, Is.EqualTo("[]"));
    }
}
