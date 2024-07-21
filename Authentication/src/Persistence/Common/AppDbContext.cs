/*
dotnet ef migrations add InitialMigration
dotnet ef database update
dotnet ef migrations remove
*/

using Microsoft.EntityFrameworkCore;
using Domain.Entities;

namespace Persistence.Common;

// TODO: Consider using dapper instead of ef
// TODO: functions that works with ef, should also be able to work with dapper for read

public sealed class AppDbContext(
    DbContextOptions<AppDbContext> options
) : DbContext(options)
{
    public DbSet<User> Users { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            // TODO: fix error when an ef command is run, environment variables are not set as expected
            // therefore, if Configurations static class is used to configure ef, the class will throw an error
            optionsBuilder.UseNpgsql(Configurations.ConnectionStrings.Postgres);
        }
        base.OnConfiguring(optionsBuilder);
    }
}
