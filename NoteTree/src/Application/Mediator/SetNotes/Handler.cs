using Application.Interfaces.Persistence;
using Domain.Models;
using ErrorOr;
using MediatR;

namespace Application.Mediator.SetNotes;

public sealed class SetNotesHandler(
    INoteReadRepository noteReadRepository,
    INoteWriteRepository noteWriteRepository)
    : IRequestHandler<SetNotesRequest, SetNotesResponse>
{
    private readonly INoteReadRepository _noteReadRepository = noteReadRepository;
    private readonly INoteWriteRepository _noteWriteRepository = noteWriteRepository;

    public async Task<SetNotesResponse> Handle(SetNotesRequest request, CancellationToken cancellationToken)
    {
        Note[]? existingNotes = await _noteReadRepository.GetByIdAsync(request.NoteRecordId);
        if (existingNotes is null)
        {
            Guid id = await _noteWriteRepository.CreateAsync(request.Notes);
            // TODO: publish id to event bus
            return new SetNotesResponse();
        }

        await _noteWriteRepository.UpdateAsync(request.NoteRecordId, request.Notes);

        return new SetNotesResponse();
    }
}
