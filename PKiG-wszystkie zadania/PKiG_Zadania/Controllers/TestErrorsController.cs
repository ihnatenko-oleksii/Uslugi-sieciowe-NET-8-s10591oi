using Microsoft.AspNetCore.Mvc;

namespace _03_Swagger_zadanie1.Controllers;

[Route("api/[controller]")]
[ApiController]
public class TestErrorsController : ControllerBase
{
    [HttpGet]
    public IActionResult Get()
    {
        throw new Exception("Testowy wyjątek");
    }
}
