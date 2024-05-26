namespace Persistence;

public static class Configurations
{
    public static readonly ConnectionStrings ConnectionStrings = new();
}

public sealed class ConnectionStrings
{
    public readonly string Postgres;

    public ConnectionStrings()
    {
        Postgres = Environment.GetEnvironmentVariable("POSTGRES_CONNECTION_STRING")
            ?? throw new ArgumentNullException("POSTGRES_CONNECTION_STRING is not set in the environment variables.");
    }
}
