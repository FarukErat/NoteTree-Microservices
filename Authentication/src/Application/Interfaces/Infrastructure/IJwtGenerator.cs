using Domain.Entities;

namespace Application.Interfaces.Infrastructure;

public interface IJwtGenerator
{
    string GenerateToken(
        User user,
        string audience);
}
