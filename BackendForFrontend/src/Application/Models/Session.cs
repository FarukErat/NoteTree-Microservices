namespace Application.Models;

public sealed record class Session(
    string UserId,
    string Token,
    string IpAddress,
    string UserAgent,
    DateTime CreatedAt,
    DateTime ExpireAt);
