using Domain.Entities;

namespace Application.Interfaces.Infrastructure;

public interface IJwtGenerator
{
    string GenerateRefreshToken(
        Guid userId,
        string audience);

    string GenerateAccessToken(
        User user,
        string audience);
}
