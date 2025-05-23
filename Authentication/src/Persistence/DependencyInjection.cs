using Application.Interfaces.Persistence;
using Persistence.Common;
using Persistence.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

// TODO: consider merging persistence and infrastructure
namespace Persistence;

public static class DependencyInjection
{
    public static IServiceCollection AddPersistence(this IServiceCollection services)
    {
        services.AddDbContextPool<AppDbContext>(options =>
        {
            if (!options.IsConfigured)
            {
                options.UseNpgsql(Configurations.ConnectionStrings.Postgres);
            }
        });

        using IServiceScope scope = services.BuildServiceProvider().CreateScope();
        AppDbContext context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        IEnumerable<string> pendingMigrations = context.Database.GetPendingMigrations();
        if (pendingMigrations.Any())
        {
            context.Database.Migrate();
        }

        services.AddScoped<IUserReadRepository, UserReadRepository>();
        services.AddScoped<IUserWriteRepository, UserWriteRepository>();

        return services;
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
