using Domain.Enums;
using ErrorOr;

namespace Application.Interfaces.Infrastructure;

public interface IJwtHelper
{
    Task<ErrorOr<Success>> VerifyTokenAsync(string token);
    Dictionary<string, dynamic> DecodeToken(string token);
    Guid? ExtractUserId(string token);
    List<Role> GetUserRoles(string token);
}
