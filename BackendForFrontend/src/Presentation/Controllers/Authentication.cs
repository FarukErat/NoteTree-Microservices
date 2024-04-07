using Dto = Presentation.DTOs;

using MediatR;
using ErrorOr;
using Microsoft.AspNetCore.Mvc;
using Application.Mediator.Register;
using Application.Mediator.Login;
using Application.Mediator.Logout;

namespace Presentation.Controllers;

[ApiController]
[Route("[controller]")]
public sealed class AuthenticationController(
    ISender sender)
    : ApiController
{
    private readonly ISender _sender = sender;
    private const string SessionIdCookieName = "SID";

    [HttpPost("register")]
    public async Task<IActionResult> Register(Dto.Register.RegisterRequest registerRequest)
    {
        RegisterRequest mediatorRequest = new(
            Username: registerRequest.Username,
            Password: registerRequest.Password,
            Email: registerRequest.Email,
            FirstName: registerRequest.FirstName,
            LastName: registerRequest.LastName);

        ErrorOr<RegisterResponse> result = await _sender.Send(mediatorRequest);

        return result.Match(
            value => Ok(new Dto.Register.RegisterResponse(value.UserId)),
            ProblemDetails);
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(Dto.Login.LoginRequest loginRequest)
    {
        LoginRequest mediatorRequest = new(
            Username: loginRequest.Username,
            Password: loginRequest.Password,
            UserAgent: HttpContext.Request.Headers.UserAgent.ToString(),
            IpAddress: HttpContext.Connection.RemoteIpAddress?.ToString());

        ErrorOr<LoginResponse> result = await _sender.Send(mediatorRequest);

        HttpContext.Response.Cookies.Append(SessionIdCookieName, result.Value.SessionId);

        return result.Match(
            value => Ok(new Dto.Login.LoginResponse(value.UserId, value.Token)),
            ProblemDetails);
    }

    [HttpGet("logout")]
    public async Task<IActionResult> Logout()
    {
        string? sessionId = Request.Cookies[SessionIdCookieName];
        if (sessionId is null)
        {
            return BadRequest("Session ID not found");
        }

        ErrorOr<LogoutResponse> result = await _sender.Send(new LogoutRequest(sessionId));

        HttpContext.Response.Cookies.Delete(SessionIdCookieName);

        return result.Match(
            _ => Ok("User logged out successfully"),
            ProblemDetails);
    }

    [HttpGet("secret")]
    public IActionResult Secret()
    {
        return Ok("secret");
    }
}
