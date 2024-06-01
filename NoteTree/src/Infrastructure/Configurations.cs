namespace Infrastructure;

public static class Configurations
{
    public const string NoteTreeAudience = "NoteTree";
    public const string Issuer = "AuthenticationServer";
    public static readonly ConnectionStrings ConnectionStrings = new();
}

public sealed class ConnectionStrings
{
    public readonly string AuthenticationUrl;

    public ConnectionStrings()
    {
        string? authentication = Environment.GetEnvironmentVariable("AUTHENTICATION_URL");
        ArgumentNullException.ThrowIfNull(authentication, "Authentication URL is required");
        AuthenticationUrl = authentication;
    }
}
