using MediatR;

namespace Application.UseCases.GetNotes;

public sealed record class GetNotesRequest(
    Guid NoteRecordId
) : IRequest<GetNotesResponse>;
