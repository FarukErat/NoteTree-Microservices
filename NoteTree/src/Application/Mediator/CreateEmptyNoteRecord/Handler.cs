using Application.Interfaces.Persistence;
using Domain.Models;
using MediatR;

namespace Application.Mediator.CreateEmptyNoteRecord;

public sealed class CreateEmptyNoteRecordHandler(
    INoteWriteRepository noteWriteRepository)
    : IRequestHandler<CreateEmptyNoteRecordRequest, CreateEmptyNoteRecordResponse>
{
    private readonly INoteWriteRepository _noteWriteRepository = noteWriteRepository;
    public async Task<CreateEmptyNoteRecordResponse> Handle(CreateEmptyNoteRecordRequest request, CancellationToken cancellationToken)
    {
        Guid id = await _noteWriteRepository.CreateAsync([]);
        return new CreateEmptyNoteRecordResponse(id);
    }
}
