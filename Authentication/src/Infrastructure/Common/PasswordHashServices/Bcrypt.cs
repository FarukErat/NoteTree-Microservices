using Application.Interfaces.Infrastructure;
using Domain.Enums;

namespace Infrastructure.Common;

public sealed class BcryptHashService : IPasswordHashService
{
    private readonly int workFactor = 12;

    public (string, PasswordHashAlgorithm) HashPassword(string password)
    {
        return (BCrypt.Net.BCrypt.HashPassword(password, workFactor), PasswordHashAlgorithm.Bcrypt);
    }

    public (bool, PasswordHashAlgorithm) VerifyPassword(string password, string passwordHash)
    {
        return (BCrypt.Net.BCrypt.Verify(password, passwordHash), PasswordHashAlgorithm.Bcrypt);
    }
}
