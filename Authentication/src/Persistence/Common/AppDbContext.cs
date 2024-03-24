/*
dotnet ef migrations add InitialMigration
dotnet ef database update
dotnet ef migrations remove
*/

using Microsoft.EntityFrameworkCore;
using Domain.Entities;

namespace Persistence.Common;

public sealed class AppDbContext : DbContext
{
    public DbSet<User> Users { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseNpgsql(Configuration.ConnectionStrings.Postgres);
        base.OnConfiguring(optionsBuilder);
    }
}
