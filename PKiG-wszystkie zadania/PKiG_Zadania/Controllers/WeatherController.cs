using System.Net;
using Microsoft.AspNetCore.Mvc;
namespace _03_Swagger_zadanie1.Controllers;
[ApiController]
[Route("weatherforecast")]
public class WeatherController(IHttpClientFactory httpClientFactory, IConfiguration config) : ControllerBase
{
    [HttpGet("{city}")]
    [Produces("application/json")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [ProducesResponseType(StatusCodes.Status502BadGateway)]
    public async Task<IActionResult> GetWeatherForecastByCity(string city)
    {
        var apiKey = config["OpenWeather:ApiKey"];
        if (string.IsNullOrWhiteSpace(apiKey))
        {
            return Problem(
                title: "Missing OpenWeather API key",
                detail: "Not found OpenWeather:ApiKey in appsettings.json.",
                statusCode: (int)HttpStatusCode.InternalServerError);
        }
        if (string.IsNullOrWhiteSpace(city))
        {
            return BadRequest("City name is required.");
        }
        var client = httpClientFactory.CreateClient("OpenWeather");
        var requestUrl = $"data/2.5/forecast?q={city}&appid={apiKey}&units=metric&lang=pl";
        HttpResponseMessage response;
        try
        {
            response = await client.GetAsync(requestUrl);
        }
        catch (HttpRequestException ex)
        {
            return Problem(
                title: "OpenWeather request failed",
                detail: ex.Message,
                statusCode: (int)HttpStatusCode.BadGateway);
        }
        if (response.StatusCode == HttpStatusCode.NotFound)
        {
            return NotFound($"City '{city}' not found in OpenWeather.");
        }
        if (!response.IsSuccessStatusCode)
        {
            var body = await response.Content.ReadAsStringAsync();
            return Problem(
                title: "OpenWeather returned an error",
                detail: $"Status: {(int)response.StatusCode} ({response.ReasonPhrase}). Body: {body}",
                statusCode: (int)HttpStatusCode.BadGateway);
        }
        var json = await response.Content.ReadAsStringAsync();
        return Content(json, "application/json");
    }
}
