using Domain.Models;

namespace Application.Interfaces.Persistence;

public interface INoteReadRepository
{
    Task<Note[]?> GetByIdAsync(Guid id);
}
