using Application.Interfaces.Persistence;
using Domain.Entities;
using Persistence.Common;

namespace Persistence.Repositories;

public sealed class UserWriteRepository(
    AppDbContext appDbContext)
    : IUserWriteRepository
{
    private readonly AppDbContext _appDbContext = appDbContext;

    public async Task<Guid> CreateAsync(User user, CancellationToken? cancellationToken = null)
    {
        await _appDbContext.Users.AddAsync(user, cancellationToken ?? CancellationToken.None);
        await _appDbContext.SaveChangesAsync(cancellationToken ?? CancellationToken.None);
        return user.Id;
    }

    public async Task UpdateAsync(User user, CancellationToken? cancellationToken = null)
    {
        _appDbContext.Users.Update(user);
        await _appDbContext.SaveChangesAsync(cancellationToken ?? CancellationToken.None);
    }
}
