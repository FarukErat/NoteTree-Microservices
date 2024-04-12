namespace Presentation.DTOs.Login;

public sealed record class LoginRequest(
    string Username,
    string Password);
