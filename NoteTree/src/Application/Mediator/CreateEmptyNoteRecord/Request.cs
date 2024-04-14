using MediatR;

namespace Application.Mediator.CreateEmptyNoteRecord;

public sealed record class CreateEmptyNoteRecordRequest
    : IRequest<CreateEmptyNoteRecordResponse>;
