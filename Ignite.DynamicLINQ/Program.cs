using Ignite.DynamicLINQ.Data;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddSingleton(new IgniteService());
builder.Services.AddScoped<CarRepository>();

var app = builder.Build();

// TODO: Add swagger and open the API page on start?
app.MapGet("/", () => "Hello World!");

app.Run();
