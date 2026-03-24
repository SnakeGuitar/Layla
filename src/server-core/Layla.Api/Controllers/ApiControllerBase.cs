using Layla.Core.Common;
using Microsoft.AspNetCore.Mvc;

namespace Layla.Api.Controllers;

[ApiController]
public abstract class ApiControllerBase : ControllerBase
{
    protected IActionResult RespondWithError(ErrorCode? errorCode) =>
        (errorCode?.GetStatusCode() ?? 500) switch
        {
            401 => Unauthorized(new { Error = errorCode?.GetMessage() }),
            403 => Forbid(),
            404 => NotFound(new { Error = errorCode?.GetMessage() }),
            409 => Conflict(new { Error = errorCode?.GetMessage() }),
            500 => StatusCode(StatusCodes.Status500InternalServerError, new { Error = errorCode?.GetMessage() }),
            _ => BadRequest(new { Error = errorCode?.GetMessage() })
        };
}
