using _03_Swagger_zadanie1.Data;
using _03_Swagger_zadanie1.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
namespace _03_Swagger_zadanie1.Controllers;
[ApiController]
[Route("cities")]
public class CityController(CitiesDbContext db) : ControllerBase
{
    [HttpPost]
    [ProducesResponseType(typeof(City), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> AddCity([FromBody] CreateCityRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Name))
        {
            return BadRequest("City name is required.");
        }
        var normalizedName = request.Name.Trim();
        var exists = await db.Cities.AnyAsync(c => c.Name.ToLower() == normalizedName.ToLower());
        if (exists)
        {
            return Conflict($"City '{normalizedName}' already exists.");
        }
        var entity = new City { Name = normalizedName };
        db.Cities.Add(entity);
        await db.SaveChangesAsync();
        return CreatedAtAction(nameof(GetCities), new { id = entity.Id }, entity);
    }
    [HttpGet]
    [ProducesResponseType(typeof(List<City>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetCities()
    {
        var cities = await db.Cities
            .OrderBy(c => c.Name)
            .ToListAsync();
        return Ok(cities);
    }
    public class CreateCityRequest
    {
        public string Name { get; set; } = string.Empty;
    }
}
