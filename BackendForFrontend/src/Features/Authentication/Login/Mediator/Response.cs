namespace Features.Authentication.Login;

public sealed record class LoginResponse(
    Guid UserId,
    Guid SessionId
);
