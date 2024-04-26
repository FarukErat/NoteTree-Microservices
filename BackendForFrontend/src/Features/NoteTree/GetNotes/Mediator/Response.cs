using Features.NoteTree.Domain.Models;

namespace Features.NoteTree.GetNotes;

public sealed record class GetNotesResponse(
    Note[] Notes
);
