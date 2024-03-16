using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace VillaAPI.Controllers;

[Controller]
[Route("api/[controller]")]
[ApiExplorerSettings(IgnoreApi = true)]
public class ErrorHandlling : ControllerBase
{
    [Route("processError")]
    public IActionResult Process([FromServices] IHostEnvironment host)
    {
        if (host.IsDevelopment())
        {
            var feature = HttpContext.Features.Get<IExceptionHandlerFeature>();
            return Problem(
                detail : feature.Error.StackTrace,
                title : feature.Error.Message,
                instance:host.EnvironmentName
                );
        }
        else
        {
            return Problem();
        }
    }
}