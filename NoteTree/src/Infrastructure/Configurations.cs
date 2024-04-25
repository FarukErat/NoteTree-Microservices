namespace Infrastructure;

public static class Configurations
{
    public static readonly ConnectionStrings ConnectionStrings = new();
}

public sealed class ConnectionStrings
{
    public readonly string AuthenticationUrl = "http://localhost:5001";
}
