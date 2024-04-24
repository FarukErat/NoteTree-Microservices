namespace Application.UseCases.Login;

// TODO: consider removing UserId from LoginResponse
public sealed record class LoginResponse(
    Guid UserId,
    string Token);
