namespace Infrastructure;

public static class Configurations
{
    public static readonly ConnectionStrings ConnectionStrings = new();
    public static readonly TimeSpan SessionDuration = TimeSpan.FromDays(7);
    public const string NoteTreeAudience = "NoteTree";
}

public sealed class ConnectionStrings
{
    public readonly string Redis;
    public readonly string AuthenticationServiceUrl;
    public readonly string NoteTreeServiceUrl;

    public ConnectionStrings()
    {
        Redis = Environment.GetEnvironmentVariable("REDIS_CONNECTION_STRING")
            ?? throw new ArgumentNullException("REDIS_CONNECTION_STRING is not set in the environment variables.");
        AuthenticationServiceUrl = Environment.GetEnvironmentVariable("AUTHENTICATION_SERVICE_URL")
            ?? throw new ArgumentNullException("AUTHENTICATION_SERVICE_URL is not set in the environment variables.");
        NoteTreeServiceUrl = Environment.GetEnvironmentVariable("NOTE_TREE_SERVICE_URL")
            ?? throw new ArgumentNullException("NOTE_TREE_SERVICE_URL is not set in the environment variables.");
    }
}
