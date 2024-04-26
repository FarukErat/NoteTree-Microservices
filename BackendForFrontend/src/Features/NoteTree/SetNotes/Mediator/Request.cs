using ErrorOr;
using Features.NoteTree.Domain.Models;
using MediatR;

namespace Features.NoteTree.SetNotes;

public sealed record class SetNotesRequest(
    Guid SessionId,
    Note[] Notes
) : IRequest<ErrorOr<SetNotesResponse>>;
