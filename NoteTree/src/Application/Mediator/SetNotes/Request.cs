using Domain.Models;
using MediatR;
using ErrorOr;

namespace Application.Mediator.SetNotes;

public sealed record class SetNotesRequest(
    Guid NoteRecordId,
    Note[] Notes)
    : IRequest<ErrorOr<SetNotesResponse>>;
