namespace Persistence;

public static class Configuration
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
        if (string.IsNullOrEmpty(postgres))
        {
            Postgres = "Host=localhost;Database=NoteTrees;Username=postgres;Password=root;Pooling=true;";
            Environment.SetEnvironmentVariable(postgresEnvironmentKey, Postgres);
        }
        else
        {
            Postgres = postgres;
        }
    }
}
