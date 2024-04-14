using Redis.OM.Modeling;

namespace Common.Models;

[Document(StorageType = StorageType.Json, IndexName = "Sessions", Prefixes = ["Session"])]
public sealed record class Session
{
    [RedisIdField]
    public Guid Id { get; set; }

    [Indexed]
    public Guid UserId { get; set; }

    public string? Token { get; set; }

    [Indexed]
    public string? IpAddress { get; set; }

    [Indexed]
    public string? UserAgent { get; set; }

    [Indexed]
    public DateTime CreatedAt { get; set; }

    [Indexed]
    public DateTime ExpireAt { get; set; }
};
