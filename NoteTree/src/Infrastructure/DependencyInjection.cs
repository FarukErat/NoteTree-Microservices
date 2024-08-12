using Application.Interfaces.Infrastructure;
using ErrorOr;
using Infrastructure.Common;
using Infrastructure.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Enrichers.Sensitive;

namespace Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services)
    {
        services.AddSingleton<IJwtHelper, JwtHelper>();
        services.AddSingleton<GetPublicKeyService>();

        return services;
    }

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
