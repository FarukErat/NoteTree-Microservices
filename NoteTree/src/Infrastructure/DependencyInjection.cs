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
        services.AddSingleton<IJwtHelper, JwtHelper>();
        services.AddSingleton<GetVerificationKeyService>();

        return services;
    }
}
