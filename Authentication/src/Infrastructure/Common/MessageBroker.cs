using MassTransit;

namespace Application.Interfaces.Infrastructure;

public sealed class MessageBroker(
    IPublishEndpoint publishEndpoint)
    : IMessageBroker
{
    private readonly IPublishEndpoint _publisher = publishEndpoint;
    public async Task PublishAsync<T>(T message, CancellationToken cancellationToken)
    {
        if (message is null)
        {
            return;
        }
        await _publisher.Publish(message, cancellationToken);
    }
}
