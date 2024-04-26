using Domain.Enums;
using ErrorOr;

namespace Application.Interfaces.Infrastructure;

public interface IJwtHelper
{
    ErrorOr<Success> VerifyToken(string token);
    Dictionary<string, dynamic> DecodeToken(string token);
    Guid? ExtractUserId(string token);
    List<Role>? GetUserRoles(string token);
}
