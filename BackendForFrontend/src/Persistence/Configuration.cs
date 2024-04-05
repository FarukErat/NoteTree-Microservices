namespace Persistence;

public static class Configurations
{
    public static readonly ConnectionStrings ConnectionStrings = new();
}

public sealed class ConnectionStrings
{
    public readonly string Redis;

    public ConnectionStrings()
    {
        string redisEnvironmentKey = "REDIS_CONNECTION_STRING";
        string? redis = Environment.GetEnvironmentVariable(redisEnvironmentKey)
            ?? "redis://localhost:6380";
        // ArgumentNullException.ThrowIfNull(redis, "Redis connection string is required");
        Redis = redis;
    }
}
