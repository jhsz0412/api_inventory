
using Delivery.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Emit;
using DeliveryEntity = Delivery.Domain.Entities.Delivery;

namespace Delivery.Infrastructure.Persistence;

public sealed class DeliveryDbContext : DbContext
{
    public DeliveryDbContext(DbContextOptions<DeliveryDbContext> options)
        : base(options)
    {
    }

    public DbSet<DeliveryEntity> Deliveries => Set<DeliveryEntity>();

    public DbSet<DeliveryItem> DeliveryItems => Set<DeliveryItem>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(
            typeof(DeliveryDbContext).Assembly);

        base.OnModelCreating(modelBuilder);
    }
}
