using ErrorOr;
using MediatR;

namespace Features.NoteTree.GetNotes;

public sealed record class GetNotesRequest(
    Guid SessionId
) : IRequest<ErrorOr<GetNotesResponse>>;
