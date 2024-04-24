using Domain.Models;
using MediatR;
using ErrorOr;

namespace Application.UseCases.SetNotes;

public sealed record class SetNotesRequest(
    Guid NoteRecordId,
    Note[] Notes
) : IRequest<ErrorOr<SetNotesResponse>>;
