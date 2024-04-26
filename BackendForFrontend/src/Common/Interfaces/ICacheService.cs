using Common.Models;

namespace Common.Interfaces;

public interface ICacheService
{
    Task<Guid> SaveSessionAsync(Session session);
    Task<Session?> GetSessionByIdAsync(Guid sessionId);
    Task<Session?> GetSessionByUserIdAsync(Guid userId);
    Task<string?> GetTokenByIdAsync(Guid sessionId);
    Task UpdateSessionByIdAsync(Guid sessionId, Session session);
    Task DeleteSessionByIdAsync(Guid sessionId);
}
