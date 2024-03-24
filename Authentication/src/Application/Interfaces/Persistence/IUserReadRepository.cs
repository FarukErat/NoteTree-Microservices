using Domain.Entities;

namespace Application.Interfaces.Persistence;

public interface IUserReadRepository
{
    Task<User?> GetByIdAsync(Guid id, CancellationToken? cancellationToken = null);

    Task<User?> GetByUsernameAsync(string userName, CancellationToken? cancellationToken = null);

    Task<User?> GetByEmailAsync(string email, CancellationToken? cancellationToken = null);
}
