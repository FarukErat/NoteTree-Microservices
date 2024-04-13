using Common;
using ErrorOr;
using Infrastructure;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Features.Authentication.Login;

public class LoginController(
    ISender sender)
    : ApiControllerBase(sender)
{
    [HttpPost]
    public async Task<IActionResult> Create(LoginRequestDto command)
    {
        ErrorOr<LoginResponse> result = await Mediator.Send(new LoginRequest(
            Username: command.Username,
            Password: command.Password,
            UserAgent: HttpContext.Request.Headers.UserAgent.ToString(),
            IpAddress: HttpContext.Connection.RemoteIpAddress?.ToString()));

        if (result.IsError)
        {
            return ProblemDetails(result.Errors);
        }

        HttpContext.Response.Cookies.Append("SID", result.Value.SessionId, new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.Strict,
            Expires = DateTime.UtcNow + Configurations.SessionDuration
        });

        return result.Match(
            response => Ok(new LoginResponseDto(
                response.UserId,
                response.Token)),
            ProblemDetails);
    }
}