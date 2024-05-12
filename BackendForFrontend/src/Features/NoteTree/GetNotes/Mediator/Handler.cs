using Common.Services;
using ErrorOr;
using Features.NoteTree.Domain.Models;
using Infrastructure;
using Infrastructure.Common;
using MediatR;

namespace Features.NoteTree.GetNotes;

public sealed class GetNotesHandler(
    GetNotesService client,
    TokenHandler tokenHandler
) : IRequestHandler<GetNotesRequest, ErrorOr<GetNotesResponse>>
{
    private readonly GetNotesService _client = client;
    private readonly TokenHandler _tokenHandler = tokenHandler;
    public async Task<ErrorOr<GetNotesResponse>> Handle(GetNotesRequest request, CancellationToken cancellationToken)
    {
        string? jwt = await _tokenHandler.GetAccessTokenBySessionIdAsync(sessionId: request.SessionId, audience: Configurations.NoteTreeAudience);
        if (jwt is null)
        {
            return Error.NotFound(description: "Token not found");
        }

        ErrorOr<Note[]> result = await _client.GetNotes(jwt);
        if (result.IsError)
        {
            return result.FirstError;
        }

        return new GetNotesResponse(Notes: result.Value);
    }
}
