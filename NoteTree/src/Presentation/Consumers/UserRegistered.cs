using Application.Mediator.CreateEmptyNoteRecord;
using Domain.Events;
using MassTransit;
using MediatR;

namespace Presentation.Consumers;

public class UserRegisteredEventConsumer(
    ISender sender
) : IConsumer<UserRegisteredEvent>
{
    private readonly ISender _sender = sender;
    public async Task Consume(ConsumeContext<UserRegisteredEvent> context)
    {
        await _sender.Send(new CreateEmptyNoteRecordRequest(context.Message.UserId));
    }
}
