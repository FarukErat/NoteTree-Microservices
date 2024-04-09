using Domain.Models;

namespace Domain.Entities;

public sealed class NoteRecord
{
    public Guid Id { get; set; }
    public Note? Note { get; set; }
}
