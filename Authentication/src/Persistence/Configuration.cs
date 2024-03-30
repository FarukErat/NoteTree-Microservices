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
        string postgresEnvironmentKey = "POSTGRES_CONNECTION_STRING";
        string? postgres = Environment.GetEnvironmentVariable(postgresEnvironmentKey);
        ArgumentNullException.ThrowIfNull(postgres, "Postgres connection string is required");
        Postgres = postgres;
    }
}
