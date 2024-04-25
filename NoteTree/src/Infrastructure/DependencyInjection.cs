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
        GetVerificationKeyService getVerificationKeyService = new();
        ErrorOr<string> result = getVerificationKeyService.GetVerificationKeyAsync();
        if (result.IsError)
        {
            throw new Exception(result.FirstError.Description);
        }
        byte[] publicKey = Convert.FromBase64String(result.Value);

        services.AddSingleton<IJwtHelper>(provider =>
            new JwtHelper(
                publicKey: publicKey
            ));

        return services;
    }
}
