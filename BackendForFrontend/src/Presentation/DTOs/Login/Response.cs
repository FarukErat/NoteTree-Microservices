namespace Presentation.DTOs.Login
{
    public sealed record class LoginResponse(
        Guid UserId,
        string Token
    );
}
