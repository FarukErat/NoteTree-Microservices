namespace Infrastructure;

public static class Configurations
{
    public static readonly ConnectionStrings ConnectionStrings = new();
    public static readonly TimeSpan SessionDuration = TimeSpan.FromDays(7);
    public static readonly string AuthenticationUrl = "http://localhost:5001";
}

public sealed class ConnectionStrings
{
    public readonly string Redis;

    public ConnectionStrings()
    {
        string redisEnvironmentKey = "REDIS_CONNECTION_STRING";
        string? redis = Environment.GetEnvironmentVariable(redisEnvironmentKey);
        ArgumentNullException.ThrowIfNull(redis, "Redis connection string is required");
        Redis = redis;
    }
}
