namespace Features.Authentication.Login;

public sealed record class LoginResponse(
    Guid UserId,
    string Token,
    Guid SessionId);
