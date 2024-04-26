using Common;
using ErrorOr;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Features.Authentication.Logout;

[Route("api/authentication")]
public class LogoutController(
    ISender sender)
    : ApiControllerBase(sender)
{
    [HttpGet("logout")]
    public async Task<IActionResult> Create()
    {
        string? sessionId = HttpContext.Request.Cookies["SID"];
        if (sessionId is null)
        {
            return BadRequest();
        }

        Guid sessionIdGuid = Guid.TryParse(sessionId, out Guid guidResult) ? guidResult : Guid.Empty;
        if (sessionIdGuid == Guid.Empty)
        {
            return BadRequest();
        }

        ErrorOr<LogoutResponse> result = await Mediator.Send(new LogoutRequest(SessionId: sessionIdGuid));
        if (result.IsError)
        {
            return ProblemDetails(result.Errors);
        }

        HttpContext.Response.Cookies.Delete("SID");

        return Ok();
    }
}
