using Application.Interfaces.Persistence;
using Microsoft.Extensions.DependencyInjection;
using Persistence.Common;

namespace Persistence;

public static class DependencyInjection
{
    public static IServiceCollection AddPersistence(this IServiceCollection services)
    {
        services.AddSingleton<ICacheService, CacheService>();
        return services;
    }
}
