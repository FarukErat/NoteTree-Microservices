using Application.Interfaces.Infrastructure;
using Infrastructure.Common;
using Infrastructure.Services;
using MassTransit;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Enrichers.Sensitive;

namespace Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services)
    {
        (string keyId, byte[] privateKey, byte[] publicKey) = KeyManager.LoadOrCreateRsaKeyPair();
        services.AddScoped<IPublicKeyProvider>(provider =>
            new PublicKeyProvider(
                currentKeyId: keyId,
                keys: KeyManager.GetPublicKeys()
            )
        );
        services.AddScoped<IJwtGenerator>(provider =>
            new JwtGenerator(
                keyId: keyId,
                privateKey: privateKey
            )
        );

        services.AddScoped<IJwtVerifier, JwtVerifier>();
        services.AddScoped<IPasswordHasherFactory, PasswordHasherFactory>();
        services.AddScoped<IMessageBroker, MessageBroker>();
        // TODO: implement outbox pattern
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

    // TODO: use loki or other log providers
    public static IHostBuilder UseInfrastructureLogging(this IHostBuilder hostBuilder)
    {
        hostBuilder.UseSerilog((context, services, configuration) =>
        {
            configuration
                .Enrich.WithSensitiveDataMasking(options =>
                {
                    options.MaskProperties.Add("password");
                    options.MaskProperties.Add("token");
                    options.MaskProperties.Add("refreshToken");
                    options.MaskProperties.Add("accessToken");
                })
                .MinimumLevel.Information()
                .WriteTo.Console();
        });

        return hostBuilder;
    }
}
