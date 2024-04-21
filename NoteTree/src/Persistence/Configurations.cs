namespace Persistence;

public static class Configurations
{
    public static readonly ConnectionStrings ConnectionStrings = new();
}

public sealed class ConnectionStrings
{
    public readonly string MongoDb = "mongodb://localhost:27017";
    public ConnectionStrings()
    {
        string mongoEnvironmentKey = "MONGODB_CONNECTION_STRING";
        string? mongo = Environment.GetEnvironmentVariable(mongoEnvironmentKey);
        ArgumentNullException.ThrowIfNull(mongo, "Mongo connection string is required");
        MongoDb = mongo;
    }
}
