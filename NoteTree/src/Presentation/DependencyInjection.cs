using MassTransit;
using Presentation.Consumers;

namespace Presentation;

public static class DependencyInjection
{
    public static void AddPresentation(this IServiceCollection services)
    {
        services.AddMassTransit(x =>
        {
            x.AddConsumersFromNamespaceContaining<UserRegisteredEventConsumer>();
            x.SetKebabCaseEndpointNameFormatter();
            x.UsingRabbitMq((context, cfg) =>
            {
                cfg.Host(Configurations.RabbitMQ.Host, "/", h =>
                {
                    h.Username(Configurations.RabbitMQ.Username);
                    h.Password(Configurations.RabbitMQ.Password);

                });
                cfg.ConfigureEndpoints(context);
            });
        });
        services.AddGrpc();
    }
}
