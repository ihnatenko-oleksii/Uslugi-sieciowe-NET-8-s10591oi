var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

var lista_kierunkow_wiatru = new[]
{
    "North", "South", "East", "West", "North-East", "North-West", "South-East", "South-West"
};

app.MapGet("/temperature", () =>
    {
        return Random.Shared.Next(-20, 55);
    })
    .WithName("GetTemperature")
    .WithOpenApi();

app.MapGet("/kierunki_wiatru", () =>
    {
        var id = Random.Shared.Next(0, lista_kierunkow_wiatru.Length);
        return lista_kierunkow_wiatru[id];
    })
    .WithName("GetKierunkiWiatru")
    .WithOpenApi();

app.Run();

// record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
// {
//     public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
// }