using Application.Interfaces.Persistence;
using Domain.Entities;
using Persistence.Common;
using Microsoft.EntityFrameworkCore;

namespace Persistence.Repositories;

public sealed class UserReadRepository(
    AppDbContext appDbContext
) : IUserReadRepository
{
    private readonly AppDbContext _appDbContext = appDbContext;

    public async Task<User?> GetByEmailAsync(string email, CancellationToken? cancellationToken = null)
    {
        return await _appDbContext.Users.FirstOrDefaultAsync(u => u.Email == email,
            cancellationToken ?? CancellationToken.None);
    }

    public async Task<User?> GetByIdAsync(Guid id, CancellationToken? cancellationToken = null)
    {
        return await _appDbContext.Users.FindAsync([id],
            cancellationToken ?? CancellationToken.None);
    }

    public async Task<User?> GetByUsernameAsync(string username, CancellationToken? cancellationToken = null)
    {
        return await _appDbContext.Users.FirstOrDefaultAsync(u => u.Username == username,
            cancellationToken ?? CancellationToken.None);
    }

    public Task<User?> GetByUsernameOrEmailAsync(string username, string email, CancellationToken? cancellationToken = null)
    {
        return _appDbContext.Users.FirstOrDefaultAsync(u => u.Username == username || u.Email == email,
            cancellationToken ?? CancellationToken.None);
    }
}
