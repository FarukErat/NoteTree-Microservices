using Microsoft.Extensions.DependencyInjection;

using Application.Interfaces.Persistence;
using Persistence.Common;

namespace Persistence;

public static class DependencyInjection
{
    public static IServiceCollection AddPersistence(this IServiceCollection services)
    {
        services.AddScoped<INoteWriteRepository, NoteWriteRepository>();
        services.AddScoped<INoteReadRepository, NoteReadRepository>();
        return services;
    }
}
