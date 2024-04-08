using Application.Interfaces.Infrastructure;
using Domain.Enums;

namespace Infrastructure.Common;

public sealed class BcryptHasher : IPasswordHasher
{
    private readonly int workFactor = 12;

    public string HashPassword(string password)
    {
        return BCrypt.Net.BCrypt.HashPassword(password, workFactor);
    }

    public bool VerifyPassword(string password, string passwordHash)
    {
        return BCrypt.Net.BCrypt.Verify(password, passwordHash);
    }
}
