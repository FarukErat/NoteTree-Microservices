using Application.Interfaces.Infrastructure;
using MassTransit;

namespace Infrastructure.Common;

public sealed class MessageBroker(
    IPublishEndpoint publishEndpoint
) : IMessageBroker
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
