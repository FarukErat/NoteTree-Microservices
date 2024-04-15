namespace Infrastructure;

public static class Configurations
{
    public static readonly JwtSettings Jwt = new();
    public static readonly RabbitMQSettings RabbitMQ = new();
}

public sealed class JwtSettings
{
    public readonly string Issuer = "AuthenticationServer";
    public readonly TimeSpan Expiry = TimeSpan.FromDays(7);
}

public sealed class RabbitMQSettings
{
    public readonly string Host;
    public readonly string Password;
    public readonly string Username;

    public RabbitMQSettings()
    {
        string rabbitMQHostEnvironmentKey = "RABBITMQ_HOST";
        string? host = Environment.GetEnvironmentVariable(rabbitMQHostEnvironmentKey);
        ArgumentNullException.ThrowIfNull(host, "RabbitMQ host is required");
        Host = host;

        string rabbitMQUsernameEnvironmentKey = "RABBITMQ_USERNAME";
        string? username = Environment.GetEnvironmentVariable(rabbitMQUsernameEnvironmentKey);
        ArgumentNullException.ThrowIfNull(username, "RabbitMQ username is required");
        Username = username;

        string rabbitMQPasswordEnvironmentKey = "RABBITMQ_PASSWORD";
        string? password = Environment.GetEnvironmentVariable(rabbitMQPasswordEnvironmentKey);
        ArgumentNullException.ThrowIfNull(password, "RabbitMQ password is required");
        Password = password;
    }
}
