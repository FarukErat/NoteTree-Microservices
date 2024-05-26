namespace Infrastructure;

public static class Configurations
{
    public static readonly JwtSettings Jwt = new();
    public static readonly RabbitMQSettings RabbitMQ = new();
}

public sealed class JwtSettings
{
    public readonly string Issuer = "AuthenticationServer";
    public readonly TimeSpan RefreshTokenExpiry = TimeSpan.FromDays(7);
    public readonly TimeSpan AccessTokenExpiry = TimeSpan.FromMinutes(3);
}

public sealed class RabbitMQSettings
{
    public readonly string Host;
    public readonly string Password;
    public readonly string Username;

    public RabbitMQSettings()
    {
        Host = Environment.GetEnvironmentVariable("RABBITMQ_HOST")
            ?? throw new ArgumentNullException("RABBITMQ_HOST is not set in the environment variables.");
        Username = Environment.GetEnvironmentVariable("RABBITMQ_USERNAME")
            ?? throw new ArgumentNullException("RABBITMQ_USERNAME is not set in the environment variables.");
        Password = Environment.GetEnvironmentVariable("RABBITMQ_PASSWORD")
            ?? throw new ArgumentNullException("RABBITMQ_PASSWORD is not set in the environment variables.");
    }
}
