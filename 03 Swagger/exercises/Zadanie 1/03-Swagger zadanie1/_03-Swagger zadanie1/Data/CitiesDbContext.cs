using _03_Swagger_zadanie1.Models;
using Microsoft.EntityFrameworkCore;
namespace _03_Swagger_zadanie1.Data;
public class CitiesDbContext(DbContextOptions<CitiesDbContext> options) : DbContext(options)
{
    public DbSet<City> Cities => Set<City>();
}
