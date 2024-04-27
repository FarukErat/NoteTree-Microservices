namespace Infrastructure;

public static class Configurations
{
    public static readonly ConnectionStrings ConnectionStrings = new();
    public static readonly TimeSpan SessionDuration = TimeSpan.FromDays(7);
}

public sealed class ConnectionStrings
{
    public readonly string Redis;
    public readonly string AuthenticationServiceUrl;
    public readonly string NoteTreeServiceUrl;

    public ConnectionStrings()
    {
        string? redis = Environment.GetEnvironmentVariable("REDIS_CONNECTION_STRING");
        ArgumentNullException.ThrowIfNull(redis, "Redis connection string is required");
        Redis = redis;

        string? authenticationServer = Environment.GetEnvironmentVariable("AUTHENTICATION_SERVICE_URL");
        ArgumentNullException.ThrowIfNull(authenticationServer, "Authentication server connection string is required");
        AuthenticationServiceUrl = authenticationServer;

        string? noteTreeServer = Environment.GetEnvironmentVariable("NOTE_TREE_SERVICE_URL");
        ArgumentNullException.ThrowIfNull(noteTreeServer, "Note tree server connection string is required");
        NoteTreeServiceUrl = noteTreeServer;
    }
}
