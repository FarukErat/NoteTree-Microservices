using Application.Models;

namespace Application.Interfaces.Persistence;

public interface ICacheService
{
    Task<string> SaveSessionAsync(Session session);
    Task<Session?> GetSessionByIdAsync(string sessionId);
    Task<Session?> GetSessionByUserIdAsync(Guid userId);
    Task UpdateSessionByIdAsync(string sessionId, Session session);
    Task DeleteSessionByIdAsync(string sessionId);
}
