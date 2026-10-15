
using Microsoft.EntityFrameworkCore;
using Planning.Domain.Entities;
using Planning.Infrastructure.Persistence.Configurations;
using System.Reflection.Emit;

namespace Planning.Infrastructure.Persistence;

public sealed class PlanningDbContext(
    DbContextOptions<PlanningDbContext> options)
    : DbContext(options)
{
    public DbSet<FiscalYear> FiscalYears => Set<FiscalYear>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(
            typeof(PlanningDbContext).Assembly);
    }
}
