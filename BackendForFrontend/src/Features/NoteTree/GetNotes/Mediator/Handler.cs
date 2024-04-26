using Common.Interfaces;
using ErrorOr;
using Features.NoteTree.Domain.Models;
using MediatR;

namespace Features.NoteTree.GetNotes;

public sealed class GetNotesHandler(
    GetNotesService client,
    ICacheService cacheService)
    : IRequestHandler<GetNotesRequest, ErrorOr<GetNotesResponse>>
{
    private readonly GetNotesService _client = client;
    private readonly ICacheService _cacheService = cacheService;
    public async Task<ErrorOr<GetNotesResponse>> Handle(GetNotesRequest request, CancellationToken cancellationToken)
    {
        string? jwt = await _cacheService.GetTokenByIdAsync(request.SessionId);
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
