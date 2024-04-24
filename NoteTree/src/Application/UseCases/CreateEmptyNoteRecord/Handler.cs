using Application.Interfaces.Persistence;
using Domain.Models;
using MediatR;

namespace Application.Mediator.CreateEmptyNoteRecord;

public sealed class CreateEmptyNoteRecordHandler(
    INoteWriteRepository noteWriteRepository
) : IRequestHandler<CreateEmptyNoteRecordRequest, CreateEmptyNoteRecordResponse>
{
    private readonly INoteWriteRepository _noteWriteRepository = noteWriteRepository;

    public async Task<CreateEmptyNoteRecordResponse> Handle(CreateEmptyNoteRecordRequest request, CancellationToken cancellationToken)
    {
        await _noteWriteRepository.CreateWithIdAsync(request.Id, []);
        return new CreateEmptyNoteRecordResponse();
    }
}
