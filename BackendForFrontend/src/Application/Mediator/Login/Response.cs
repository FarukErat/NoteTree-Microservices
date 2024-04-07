namespace Application.Mediator.Login;

public sealed record class LoginResponse(
    Guid UserId,
    string Token,
    string SessionId);
