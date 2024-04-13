using Common.Interfaces;
using Infrastructure.Common;

namespace Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddPersistence(this IServiceCollection services)
    {
        services.AddSingleton<ICacheService, CacheService>();
        return services;
    }
}
