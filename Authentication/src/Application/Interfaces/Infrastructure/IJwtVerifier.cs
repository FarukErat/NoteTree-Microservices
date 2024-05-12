using ErrorOr;

namespace Application.Interfaces.Infrastructure;

public interface IJwtVerifier
{
    ErrorOr<Success> VerifyToken(string token, string audience);

    Guid? ExtractUserId(string token);
}
