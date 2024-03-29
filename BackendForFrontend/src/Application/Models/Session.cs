namespace Application.Models;

public sealed record class Session(
    string UserId,
    string IpAddress,
    string UserAgent,
    DateTime CreatedAt,
    DateTime ExpireAt);
