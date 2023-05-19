using Ignite.DynamicLINQ.Data;
using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddSingleton(new IgniteService());
builder.Services.AddScoped<CarRepository>();

var app = builder.Build();

// TODO: Add swagger and open the API page on start?
app.MapGet("/", () => "Hello World!");

app.MapGet(
    "/cars",
           (string? make,
            string? model,
            int? year,
            SearchMode? searchMode,
            string? columns,
            QueryMode? queryMode,
            [FromServices] CarRepository repo) =>
               queryMode switch
               {
                   QueryMode.Linq => repo.GetCarsLinq(make, model, year, searchMode ?? SearchMode.All, columns?.Split(',')),
                   QueryMode.LinqDynamic => repo.GetCarsLinqDynamic(make, model, year, searchMode ?? SearchMode.All, columns?.Split(',')),
                   _ => repo.GetCarsSql(make, model, year, searchMode ?? SearchMode.All, columns?.Split(','))
               });

app.Run();
