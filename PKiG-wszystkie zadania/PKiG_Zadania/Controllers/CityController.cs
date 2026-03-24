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
        return CreatedAtAction(nameof(GetCityById), new { id = entity.Id }, entity);
    }

    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(City), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetCityById([FromRoute] int id)
    {
        var city = await db.Cities.FindAsync(id);
        if (city is null)
        {
            return NotFound($"City with id '{id}' was not found.");
        }

        return Ok(city);
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

    [HttpPut("{id:int}")]
    [ProducesResponseType(typeof(City), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> UpdateCity([FromRoute] int id, [FromBody] UpdateCityRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Name))
        {
            return BadRequest("City name is required.");
        }

        var city = await db.Cities.FindAsync(id);
        if (city is null)
        {
            return NotFound($"City with id '{id}' was not found.");
        }

        var normalizedName = request.Name.Trim();
        var duplicateExists = await db.Cities.AnyAsync(c => c.Id != id && c.Name.ToLower() == normalizedName.ToLower());
        if (duplicateExists)
        {
            return Conflict($"City '{normalizedName}' already exists.");
        }

        city.Name = normalizedName;
        await db.SaveChangesAsync();

        return Ok(city);
    }

    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteCity([FromRoute] int id)
    {
        var city = await db.Cities.FindAsync(id);
        if (city is null)
        {
            return NotFound($"City with id '{id}' was not found.");
        }

        db.Cities.Remove(city);
        await db.SaveChangesAsync();

        return NoContent();
    }

    public class CreateCityRequest
    {
        public string Name { get; set; } = string.Empty;
    }

    public class UpdateCityRequest
    {
        public string Name { get; set; } = string.Empty;
    }
}
