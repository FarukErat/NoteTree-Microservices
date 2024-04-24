using Features.Authentication.Login;
using Features.Authentication.Register;

namespace Features;

public static class DependencyInjection
{
    public static IServiceCollection AddFeatures(this IServiceCollection services)
    {
        services.AddSingleton<RegisterService>();
        services.AddSingleton<LoginService>();
        return services;
    }
}
