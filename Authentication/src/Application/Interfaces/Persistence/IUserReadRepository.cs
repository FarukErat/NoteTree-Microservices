using Domain.Entities;

namespace Application.Interfaces.Persistence;

public interface IUserReadRepository
{
    Task<User?> GetByIdAsync(Guid id, CancellationToken? cancellationToken = null);

    Task<User?> GetByUsernameAsync(string username, CancellationToken? cancellationToken = null);

    Task<User?> GetByEmailAsync(string email, CancellationToken? cancellationToken = null);
    Task<User?> GetByUsernameOrEmailAsync(string username, string email, CancellationToken? cancellationToken = null);
}
