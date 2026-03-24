using _03_Swagger_zadanie1.Data;
using Microsoft.EntityFrameworkCore;
using Serilog;
var builder = WebApplication.CreateBuilder(args);
builder.Host.UseSerilog((hostingContext, loggerConfiguration) => loggerConfiguration
    .ReadFrom.Configuration(hostingContext.Configuration));

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<CitiesDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("CitiesDb")));

builder.Services.AddHttpClient("OpenWeather", client =>
{
    client.BaseAddress = new Uri("https://api.openweathermap.org/");
});
var app = builder.Build();
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<CitiesDbContext>();
    db.Database.EnsureCreated();
}
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    // app.UseExceptionHandler("/api/error");
    app.UseSwagger();
    app.UseSwaggerUI();
}
else
{
    app.UseExceptionHandler("/api/error");
}
app.UseHttpsRedirection();
app.MapControllers();
app.Run();
