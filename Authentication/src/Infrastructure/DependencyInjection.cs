using System.Security.Cryptography;
using Application.Interfaces.Infrastructure;
using Infrastructure.Common;
using Infrastructure.Services;
using MassTransit;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services)
    {
        (string keyId, byte[] privateKey, byte[] publicKey) = KeyManager.LoadOrCreateRsaKeyPair();
        services.AddScoped<IJwtGenerator>(provider =>
            new JwtGenerator(
                keyId,
                privateKey,
                Configurations.Jwt.Issuer,
                Configurations.Jwt.Expiry
            )
        );
        services.AddScoped<IPublicKeyProvider>(provider =>
            new PublicKeyProvider(
                keyId,
                KeyManager.GetPublicKeys()
            )
        );

        services.AddScoped<IPasswordHasherFactory, PasswordHasherFactory>();
        services.AddScoped<IMessageBroker, MessageBroker>();
        services.AddMassTransit(x =>
        {
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

        return services;
    }
}
