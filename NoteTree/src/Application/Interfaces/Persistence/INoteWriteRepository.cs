using Domain.Models;

namespace Application.Interfaces.Persistence;

public interface INoteWriteRepository
{
    Task CreateWithIdAsync(Guid Id, Note[] note);
    Task UpdateAsync(Guid id, Note[] note);
    Task DeleteAsync(Guid id);
}
