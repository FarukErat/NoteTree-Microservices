using Domain.Entities;

namespace Application.Interfaces.Persistence;

public interface IUserWriteRepository
{
    Task<Guid> CreateAsync(User user, CancellationToken? cancellationToken = null);

    Task UpdateAsync(User user, CancellationToken? cancellationToken = null);
}
