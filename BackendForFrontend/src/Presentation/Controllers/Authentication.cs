using Application.Dtos;
using Application.Interfaces.Infrastructure;
using Application.Interfaces.Persistence;
using Application.Models;
using ErrorOr;
using Microsoft.AspNetCore.Mvc;

namespace Presentation.Controllers;

[ApiController]
[Route("[controller]")]
public sealed class AuthenticationController(
    IAuthenticationService authenticationService,
    ICacheService cacheService)
    : ApiController
{
    private readonly IAuthenticationService _authenticationService = authenticationService;
    private readonly ICacheService _cacheService = cacheService;

    [HttpPost("register")]
    public IActionResult Register(RegisterRequest registerRequest)
    {
        ErrorOr<RegisterResponse> result = _authenticationService.Register(registerRequest);
        return result.Match(
            Ok,
            ProblemDetails);
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginRequest loginRequest)
    {
        ErrorOr<LoginResponse> result = await _authenticationService.Login(loginRequest, HttpContext);
        return result.Match(
            Ok,
            ProblemDetails);
    }

    [HttpGet("logout")]
    public IActionResult Logout()
    {
        _authenticationService.Logout(HttpContext);
        return Ok(new { message = "User logged out successfully!" });
    }

    [HttpGet("secret")]
    public IActionResult Secret()
    {
        return Ok("secret");
    }
}
