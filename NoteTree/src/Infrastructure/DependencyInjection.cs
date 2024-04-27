using Application.Interfaces.Infrastructure;
using ErrorOr;
using Infrastructure.Common;
using Infrastructure.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services)
    {
        byte[]? publicKey = GetPublicKeyFromFile();

        if (publicKey is null)
        {
            GetVerificationKeyService getVerificationKeyService = new();
            ErrorOr<byte[]> result = getVerificationKeyService.GetVerificationKey();
            if (result.IsError)
            {
                throw new Exception(result.FirstError.Description);
            }
            publicKey = result.Value;
            SavePublicKeyToFile(publicKey);
        }

        services.AddSingleton<IJwtHelper>(provider =>
            new JwtHelper(
                publicKey: publicKey
            ));

        return services;
    }

    private static byte[]? GetPublicKeyFromFile()
    {
        if (!Directory.Exists(Directories.KeyFolder))
        {
            return null;
        }

        if (!File.Exists(Directories.PublicKeyFilePath))
        {
            return null;
        }

        byte[] publicKey = File.ReadAllBytes(Directories.PublicKeyFilePath);

        return publicKey;
    }

    private static void SavePublicKeyToFile(byte[] publicKey)
    {
        if (!Directory.Exists(Directories.KeyFolder))
        {
            Directory.CreateDirectory(Directories.KeyFolder);
        }

        File.WriteAllBytes(Directories.PublicKeyFilePath, publicKey);
    }
}

static class Directories
{
    public static string CurrentDir => Directory.GetCurrentDirectory();
    public static string ParentDir => Directory.GetParent(CurrentDir)?.FullName ?? CurrentDir;
    public static string KeyFolder => Path.Combine(ParentDir, "Keys");
    public static string PublicKeyFilePath => Path.Combine(KeyFolder, "public_key.pem");
}
