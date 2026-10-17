
using Microsoft.EntityFrameworkCore;
using Navigation.Domain.Entities;

namespace Navigation.Infrastructure.Persistence;

public sealed class NavigationDbContext(
    DbContextOptions<NavigationDbContext> options) : DbContext(options)
{
    public DbSet<MenuItem> MenuItems => Set<MenuItem>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(
            typeof(NavigationDbContext).Assembly);
    }
}
