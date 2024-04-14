using Application.Interfaces.Persistence;
using Domain.Models;
using MediatR;

namespace Application.Mediator.GetNotes;

public sealed class GetNotesHandler(
    INoteReadRepository noteReadRepository)
    : IRequestHandler<GetNotesRequest, GetNotesResponse>
{
    private readonly INoteReadRepository _noteReadRepository = noteReadRepository;
    public async Task<GetNotesResponse> Handle(GetNotesRequest request, CancellationToken cancellationToken)
    {
        Note[]? notes = await _noteReadRepository.GetByIdAsync(request.NoteRecordId);
        return new GetNotesResponse(notes ?? []);
    }
}
