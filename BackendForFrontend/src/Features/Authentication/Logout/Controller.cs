using Common;
using ErrorOr;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Features.Authentication.Logout;

public class LogoutController(
    ISender sender)
    : ApiControllerBase(sender)
{
    [HttpGet]
    public async Task<IActionResult> Create()
    {
        ErrorOr<LogoutResponse> result = await Mediator.Send(new LogoutRequest(
            SessionId: HttpContext.Request.Cookies["SID"]
        ));
        if (result.IsError)
        {
            return ProblemDetails(result.Errors);
        }
        HttpContext.Response.Cookies.Delete("SID");
        return Ok();
    }
}
