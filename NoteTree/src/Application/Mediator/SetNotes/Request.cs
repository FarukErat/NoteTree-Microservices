using Domain.Models;
using MediatR;

namespace Application.Mediator.SetNotes;

public sealed record class SetNotesRequest(
    Guid NoteRecordId,
    Note[] Notes)
    : IRequest<SetNotesResponse>;
