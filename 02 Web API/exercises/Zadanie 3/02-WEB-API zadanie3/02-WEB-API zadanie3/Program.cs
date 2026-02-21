using System.Net;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddHttpClient("OpenWeather", client =>
{
    client.BaseAddress = new Uri("https://api.openweathermap.org/");
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapGet("/weatherforecast/{city}", async (
        string city,
        IHttpClientFactory httpClientFactory,
        IConfiguration config) =>
    {
        var apiKey = config["OpenWeather:ApiKey"];

        if (string.IsNullOrWhiteSpace(apiKey))
        {
            return Results.Problem(
                title: "Missing OpenWeather API key",
                detail: "Not found OpenWeather:ApiKey in appsettings.json.",
                statusCode: (int)HttpStatusCode.InternalServerError);
        }

        if (string.IsNullOrWhiteSpace(city))
        {
            return Results.BadRequest("City name is required.");
        }

        var client = httpClientFactory.CreateClient("OpenWeather");

        var requestUrl =
            $"data/2.5/forecast?q={city}&appid={apiKey}&units=metric&lang=pl";

        HttpResponseMessage response;
        try
        {
            response = await client.GetAsync(requestUrl);
        }
        catch (HttpRequestException ex)
        {
            return Results.Problem(
                title: "OpenWeather request failed",
                detail: ex.Message,
                statusCode: (int)HttpStatusCode.BadGateway);
        }

        if (response.StatusCode == HttpStatusCode.NotFound)
        {
            return Results.NotFound($"City '{city}' not found in OpenWeather.");
        }

        if (!response.IsSuccessStatusCode)
        {
            var body = await response.Content.ReadAsStringAsync();
            return Results.Problem(
                title: "OpenWeather returned an error",
                detail: $"Status: {(int)response.StatusCode} ({response.ReasonPhrase}). Body: {body}",
                statusCode: (int)HttpStatusCode.BadGateway);
        }

        return Results.Content(await response.Content.ReadAsStringAsync(), "application/json");
    })
    .WithName("GetWeatherForecastByCity")
    .WithOpenApi();

app.Run();