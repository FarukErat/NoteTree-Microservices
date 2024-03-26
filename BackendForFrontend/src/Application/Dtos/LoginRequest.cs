namespace Application.Dtos;

public sealed record class LoginRequest(
    string Username,
    string Password);
