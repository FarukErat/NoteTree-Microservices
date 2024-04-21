using MediatR;

namespace Application.Mediator.GetNotes;

public sealed record class GetNotesRequest(
    Guid NoteRecordId
) : IRequest<GetNotesResponse>;
