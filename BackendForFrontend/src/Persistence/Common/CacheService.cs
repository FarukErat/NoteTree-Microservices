using System.Text;
using System.Text.Json;
using Application.Interfaces.Persistence;
using Application.Models;
using StackExchange.Redis;

namespace Persistence.Common;

public class CacheService : ICacheService
{
    private readonly IDatabase _database;

    public CacheService()
    {
        ConnectionMultiplexer redisConnection = ConnectionMultiplexer.Connect(
            Configurations.ConnectionStrings.Redis);
        _database = redisConnection.GetDatabase();
    }

    public async Task DeleteSessionByIdAsync(string sessionId)
    {
        await _database.KeyDeleteAsync(sessionId);
    }

    public async Task<Session?> GetSessionByIdAsync(string sessionId)
    {
        string? sessionJson = await _database.StringGetAsync(sessionId);

        if (sessionJson == null)
        {
            return null;
        }

        return JsonSerializer.Deserialize<Session>(sessionJson);
    }

    public async Task<string> SaveSessionAsync(Session session)
    {
        string sessionId = Convert.ToBase64String(
            Guid.NewGuid().ToByteArray())
                .Replace('/', '_')
                .Replace('+', '-')
                .TrimEnd('=');
        string sessionJson = JsonSerializer.Serialize(session);
        await _database.StringSetAsync(sessionId, sessionJson, TimeSpan.FromMinutes(30));
        return sessionId;
    }

    public async Task UpdateSessionByIdAsync(string sessionId, Session session)
    {
        string sessionJson = JsonSerializer.Serialize(session);
        await _database.StringSetAsync(sessionId, sessionJson, TimeSpan.FromMinutes(30));
    }
}
