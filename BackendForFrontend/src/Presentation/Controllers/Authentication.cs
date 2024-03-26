using Application.Dtos;
using Application.Interfaces.Infrastructure;
using ErrorOr;
using Microsoft.AspNetCore.Mvc;

namespace Presentation.Controllers;

[ApiController]
[Route("[controller]")]
public sealed class AuthenticationController(
    IAuthenticationService authenticationService)
    : ApiController
{
    private readonly IAuthenticationService _authenticationService = authenticationService;

    [HttpPost("register")]
    public IActionResult Register(RegisterRequest registerRequest)
    {
        ErrorOr<Success> result = _authenticationService.Register(registerRequest);
        return result.Match(
            success => Ok(new { message = "User registered successfully!" }),
            ProblemDetails);
    }

    [HttpPost("login")]
    public IActionResult Login(LoginRequest loginRequest)
    {
        ErrorOr<string> result = _authenticationService.Login(loginRequest);
        return result.Match(
            token => Ok(new { token }),
            ProblemDetails);
    }

    [HttpGet("logout")]
    public IActionResult Logout()
    {
        return Ok("logout");
    }

    [HttpGet("secret")]
    public IActionResult Secret()
    {
        return Ok("secret");
    }
}
