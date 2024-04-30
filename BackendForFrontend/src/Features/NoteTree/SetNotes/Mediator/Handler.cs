using Common.Interfaces;
using ErrorOr;
using Features.NoteTree.Domain.Models;
using MediatR;

namespace Features.NoteTree.SetNotes;

public sealed class SetNotesHandler(
    SetNotesService client,
    ICacheService cacheService
) : IRequestHandler<SetNotesRequest, ErrorOr<SetNotesResponse>>
{
    private readonly SetNotesService _client = client;
    private readonly ICacheService _cacheService = cacheService;
    public async Task<ErrorOr<SetNotesResponse>> Handle(SetNotesRequest request, CancellationToken cancellationToken)
    {
        string? jwt = await _cacheService.GetTokenByIdAsync(request.SessionId);
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
