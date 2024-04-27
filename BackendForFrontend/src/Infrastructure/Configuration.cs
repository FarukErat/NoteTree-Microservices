namespace Infrastructure;

public static class Configurations
{
    public static readonly ConnectionStrings ConnectionStrings = new();
    public static readonly TimeSpan SessionDuration = TimeSpan.FromDays(7);

    // TODO: move AuthenticationUrl to ConnectionStrings
    public static readonly string AuthenticationUrl = "http://localhost:5101";
    public static readonly string NoteTreeServiceUrl = "http://localhost:5103";
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
