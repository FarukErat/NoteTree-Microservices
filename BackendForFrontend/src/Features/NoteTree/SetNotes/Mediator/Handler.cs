using Common.Services;
using ErrorOr;
using Infrastructure;
using MediatR;

namespace Features.NoteTree.SetNotes;

public sealed class SetNotesHandler(
    SetNotesService client,
    TokenHandler tokenHandler
) : IRequestHandler<SetNotesRequest, ErrorOr<SetNotesResponse>>
{
    private readonly SetNotesService _client = client;
    private readonly TokenHandler _tokenHandler = tokenHandler;
    public async Task<ErrorOr<SetNotesResponse>> Handle(SetNotesRequest request, CancellationToken cancellationToken)
    {
        string? jwt = await _tokenHandler.GetAccessTokenBySessionIdAsync(request.SessionId, Configurations.NoteTreeAudience);
        if (jwt is null)
        {
            return Error.NotFound(description: "Token not found");
        }

        ErrorOr<Success> result = await _client.SetNotes(jwt, request.Notes);
        if (result.IsError)
        {
            return result.FirstError;
        }

        return new SetNotesResponse();
    }
}
