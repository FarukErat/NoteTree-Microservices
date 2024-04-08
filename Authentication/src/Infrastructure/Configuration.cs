using Domain.Enums;

namespace Infrastructure;

public static class Configurations
{
    public static readonly JwtSettings JwtSettings = new();
}

public sealed class JwtSettings
{
    public readonly string Issuer = "AuthenticationServer";
    public readonly TimeSpan Expiry = TimeSpan.FromDays(7);
}
