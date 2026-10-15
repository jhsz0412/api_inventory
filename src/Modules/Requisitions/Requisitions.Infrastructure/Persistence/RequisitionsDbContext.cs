using Microsoft.EntityFrameworkCore;
using Requisitions.Domain.Entities;

namespace Requisitions.Infrastructure.Persistence;

public sealed class RequisitionsDbContext : DbContext
{
    public RequisitionsDbContext(
        DbContextOptions<RequisitionsDbContext> options)
        : base(options)
    {
    }

    public DbSet<Requisition> Requisitions =>
        Set<Requisition>();

    public DbSet<RequisitionItem> RequisitionItems =>
        Set<RequisitionItem>();

    public DbSet<RequisitionNumberSequence> RequisitionNumberSequences =>
        Set<RequisitionNumberSequence>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(
            typeof(RequisitionsDbContext).Assembly);
    }
}