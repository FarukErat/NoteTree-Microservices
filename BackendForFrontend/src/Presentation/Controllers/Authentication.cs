using Application.Interfaces.Infrastructure;
using Microsoft.AspNetCore.Mvc;

namespace Presentation.Controllers;

[ApiController]
[Route("[controller]")]
public sealed class AuthenticationController(
    IAuthenticationService authenticationService)
    : ControllerBase
{
    private readonly IAuthenticationService _authenticationService = authenticationService;

    [HttpGet("register")]
    public IActionResult Register()
    {
        return Ok("Register");
    }

    [HttpGet("login")]
    public IActionResult Login()
    {
        return Ok("login");
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
