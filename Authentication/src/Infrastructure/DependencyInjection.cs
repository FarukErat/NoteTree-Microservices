using System.Security.Cryptography;
using Application.Interfaces.Infrastructure;
using Infrastructure.Common;
using MassTransit;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services)
    {
        // prepare asymmetric key pair
        (byte[]? privateKey, byte[]? publicKey) = GetRsaKeyPairFromFile();
        if ((privateKey is null) || (publicKey is null))
        {
            (privateKey, publicKey) = GenerateRsaKeyPair();
            SaveRsaKeyPairToFile(privateKey, publicKey);
        }

        services.AddSingleton<IRsaKeyPair>(provider =>
            new RsaKeyPair(privateKey, Convert.ToBase64String(publicKey)));

        services.AddScoped<IPasswordHasherFactory, PasswordHasherFactory>();

        services.AddScoped<IJwtGenerator>(provider =>
            new JwtGenerator(
                privateKey,
                Configurations.JwtSettings.Issuer,
                Configurations.JwtSettings.Expiry
            )
        );

        services.AddScoped<IMessageBroker, MessageBroker>();
        services.AddMassTransit(x =>
        {
            x.SetKebabCaseEndpointNameFormatter();
            x.UsingRabbitMq((context, cfg) =>
            {
                cfg.Host("localhost", "/", h =>
                {
                    h.Username("guest");
                    h.Password("guest");
                });
                cfg.ConfigureEndpoints(context);
            });
        });

        return services;
    }

    private static (byte[]? privateKey, byte[]? publicKey) GetRsaKeyPairFromFile()
    {
        if (!Directory.Exists(Directories.KeyFolder))
        {
            return (null, null);
        }

        if ((!File.Exists(Directories.PrivateKeyFilePath))
            || (!File.Exists(Directories.PublicKeyFilePath)))
        {
            return (null, null);
        }

        byte[] privateKey = File.ReadAllBytes(Directories.PrivateKeyFilePath);
        byte[] publicKey = File.ReadAllBytes(Directories.PublicKeyFilePath);

        return (privateKey, publicKey);
    }

    private static void SaveRsaKeyPairToFile(byte[] privateKey, byte[] publicKey)
    {
        if (!Directory.Exists(Directories.KeyFolder))
        {
            Directory.CreateDirectory(Directories.KeyFolder);
        }

        File.WriteAllBytes(Directories.PrivateKeyFilePath, privateKey);
        File.WriteAllBytes(Directories.PublicKeyFilePath, publicKey);
    }

    private static (byte[] privateKey, byte[] publicKey) GenerateRsaKeyPair()
    {
        using RSA rsa = RSA.Create();
        byte[] privateKey = rsa.ExportRSAPrivateKey();
        byte[] publicKey = rsa.ExportRSAPublicKey();

        return (privateKey, publicKey);
    }
}

static class Directories
{
    public static string CurrentDir => Directory.GetCurrentDirectory();
    public static string ParentDir => Directory.GetParent(CurrentDir)?.FullName ?? CurrentDir;
    public static string KeyFolder => Path.Combine(ParentDir, "Keys");
    public static string PrivateKeyFilePath => Path.Combine(KeyFolder, "private_key.pem");
    public static string PublicKeyFilePath => Path.Combine(KeyFolder, "public_key.pem");
}
