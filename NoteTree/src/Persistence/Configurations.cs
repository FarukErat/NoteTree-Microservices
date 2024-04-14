namespace Persistence;

public static class Configurations
{
    public static readonly ConnectionStrings ConnectionStrings = new();
}

public sealed class ConnectionStrings
{
    public readonly string MongoDbConnectionString = "mongodb://localhost:27017";
}
