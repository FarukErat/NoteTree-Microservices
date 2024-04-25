namespace Application.UseCases.Login;

public sealed record class LoginResponse(
    Guid UserId,
    string Token);
