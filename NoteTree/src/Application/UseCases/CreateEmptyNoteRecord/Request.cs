using MediatR;

namespace Application.Mediator.CreateEmptyNoteRecord;

public sealed record class CreateEmptyNoteRecordRequest(
    Guid Id
) : IRequest<CreateEmptyNoteRecordResponse>;
