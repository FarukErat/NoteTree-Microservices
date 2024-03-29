namespace Application.Dtos;

public sealed record class LoginResponse(
    Guid UserId,
    string Token);
