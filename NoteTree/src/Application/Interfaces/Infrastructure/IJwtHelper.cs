using ErrorOr;

namespace Application.Interfaces.Infrastructure;

public interface IJwtHelper
{
    ErrorOr<Success> VerifyToken(string token);
    Dictionary<string, dynamic> DecodeToken(string token);
    ErrorOr<Guid> ExtractUserId(string token);
}
