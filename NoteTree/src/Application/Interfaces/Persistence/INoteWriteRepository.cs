using Domain.Models;

namespace Application.Interfaces.Persistence;

public interface INoteWriteRepository
{
    Task<Guid> CreateAsync(Note[] note);
    Task UpdateAsync(Guid id, Note[] note);
    Task DeleteAsync(Guid id);
}
