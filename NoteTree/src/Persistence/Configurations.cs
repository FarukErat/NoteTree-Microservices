namespace Persistence;

public static class Configurations
{
    public static readonly ConnectionStrings ConnectionStrings = new();
}

public sealed class ConnectionStrings
{
    public readonly string MongoDb;
    public ConnectionStrings()
    {
        string? mongoDb = Environment.GetEnvironmentVariable("MONGODB_CONNECTION_STRING");
        ArgumentNullException.ThrowIfNull(mongoDb, "Mongo connection string is required");
        MongoDb = mongoDb;
    }
}
