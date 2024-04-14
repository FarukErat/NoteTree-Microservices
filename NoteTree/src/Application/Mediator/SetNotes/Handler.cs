using Application.Interfaces.Persistence;
using Domain.Models;
using ErrorOr;
using MediatR;

namespace Application.Mediator.SetNotes;

public sealed class SetNotesHandler(
    INoteReadRepository noteReadRepository,
    INoteWriteRepository noteWriteRepository)
    : IRequestHandler<SetNotesRequest, ErrorOr<SetNotesResponse>>
{
    private readonly INoteReadRepository _noteReadRepository = noteReadRepository;
    private readonly INoteWriteRepository _noteWriteRepository = noteWriteRepository;

    public async Task<ErrorOr<SetNotesResponse>> Handle(SetNotesRequest request, CancellationToken cancellationToken)
    {
        Note[]? existingNotes = await _noteReadRepository.GetByIdAsync(request.NoteRecordId);
        if (existingNotes is null)
        {
            return Error.NotFound(description: "Note record not found");
        }

        await _noteWriteRepository.UpdateAsync(request.NoteRecordId, request.Notes);

        return new SetNotesResponse();
    }
}
