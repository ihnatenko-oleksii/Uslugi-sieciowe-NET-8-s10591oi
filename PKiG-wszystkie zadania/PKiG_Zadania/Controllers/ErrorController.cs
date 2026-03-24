using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace _03_Swagger_zadanie1.Controllers;

[ApiController]
public class ErrorController : ControllerBase
{
    [Route("/api/error")]
    [ApiExplorerSettings(IgnoreApi = true)]
    public IActionResult HandleError()
    {
        var exceptionFeature = HttpContext.Features.Get<IExceptionHandlerFeature>();
        if (exceptionFeature is not null)
        {
            Console.WriteLine(exceptionFeature.Error);
            return Problem(detail: exceptionFeature.Error.Message, title: "Wystąpił błąd");
        }

        return Problem();
    }
}
