namespace Application.Features.Login;

public sealed record class LoginResponse(
    Guid UserId,
    string Token);
