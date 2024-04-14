using Redis.OM.Searching;
using Redis.OM;
using Common.Interfaces;
using Common.Models;

namespace Infrastructure.Common;

public class CacheService : ICacheService
{
    private readonly IRedisCollection<Session> _sessions;

    public CacheService()
    {
        RedisConnectionProvider provider = new(Configurations.ConnectionStrings.Redis);
        provider.Connection.CreateIndex(typeof(Session));
        _sessions = provider.RedisCollection<Session>();
    }

    public async Task<Guid> SaveSessionAsync(Session session)
    {
        session.Id = Guid.NewGuid();
        string rawId = await _sessions
            .InsertAsync(session, session.ExpireAt - DateTime.UtcNow);
        string id = rawId.Split(':')[1];
        Guid result = Guid.Parse(id);
        return result;
    }

    public async Task<Session?> GetSessionByIdAsync(Guid sessionId)
    {
        return await _sessions
            .FindByIdAsync(sessionId.ToString());
    }

    public async Task<Session?> GetSessionByUserIdAsync(Guid userId)
    {
        return await _sessions
            .Where(
                x => x.UserId == userId)
            .FirstOrDefaultAsync();
    }

    public async Task UpdateSessionByIdAsync(Guid sessionId, Session session)
    {
        session.Id = sessionId; // for clarity
        await _sessions.UpdateAsync(session);
    }

    public async Task DeleteSessionByIdAsync(Guid sessionId)
    {
        Session? session = await _sessions.FindByIdAsync(sessionId.ToString());
        if (session is not null)
        {
            await _sessions.DeleteAsync(session);
        }
    }
}
