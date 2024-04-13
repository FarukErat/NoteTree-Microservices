namespace Features.Authentication.Login;

public sealed record class LoginRequestDto(
    string Username,
    string Password);
