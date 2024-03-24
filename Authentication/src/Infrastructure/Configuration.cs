using Domain.Enums;

namespace Infrastructure;

public static class Configuration
{
    public static readonly JwtSettings JwtSettings = new();
    public static readonly PasswordHashAlgorithm DefaultPasswordHashAlgorithm = PasswordHashAlgorithm.Argon2id;
}

public sealed class JwtSettings
{
    public readonly string Issuer = "AuthenticationServer";
    public readonly TimeSpan Expiry = TimeSpan.FromDays(7);
}
