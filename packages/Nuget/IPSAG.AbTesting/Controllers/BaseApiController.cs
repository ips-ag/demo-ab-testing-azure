using IPSAG.AbTesting.ResponseModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace IPSAG.AbTesting.Controllers;

[ApiController]
[Produces("application/json")]
[Consumes("application/json")]
[ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
[ProducesResponseType(StatusCodes.Status401Unauthorized)]
[ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status403Forbidden)]
[ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status500InternalServerError)]
public class BaseApiController : ControllerBase
{
    protected internal new IActionResult NotFound()
    {
        var ti = HttpContext.TraceIdentifier;
        var result = new ApiErrorResponse("The requested resource was not found", "There is no current representation for the target resource", ti);
        return new ObjectResult(result)
        {
            StatusCode = StatusCodes.Status404NotFound
        };
    }

    protected internal IActionResult Forbidden()
    {
        var ti = HttpContext.TraceIdentifier;
        var result = new ApiErrorResponse("Requesting resource is forbidden", "Unauthorized resource access", ti);
        return new ObjectResult(result)
        {
            StatusCode = StatusCodes.Status403Forbidden
        };
    }

    protected internal IActionResult ServerError(Exception exception)
    {
        var ti = HttpContext.TraceIdentifier;
        var result = new ApiErrorResponse("Something unexpected happened", exception.Message, ti);
        return new ObjectResult(result)
        {
            StatusCode = StatusCodes.Status500InternalServerError
        };
    }
}