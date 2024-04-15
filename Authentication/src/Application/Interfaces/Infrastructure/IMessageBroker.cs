namespace Application.Interfaces.Infrastructure;

public interface IMessageBroker
{
    Task PublishAsync<T>(T message, CancellationToken cancellationToken);
}
