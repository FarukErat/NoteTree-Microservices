/*
dotnet ef migrations add InitialMigration
dotnet ef database update
dotnet ef migrations remove
*/

using Microsoft.EntityFrameworkCore;
using Domain.Entities;

namespace Persistence.Common;

// TODO: Consider using dapper instead of ef
// TODO: functions that works with ef, should also be able to work with dapper

public sealed class AppDbContext(
    DbContextOptions<AppDbContext> options
) : DbContext(options)
{
    public DbSet<User> Users { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            optionsBuilder.UseNpgsql(Configurations.ConnectionStrings.Postgres);
        }
        base.OnConfiguring(optionsBuilder);
    }
}
