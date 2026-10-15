
using Inventory.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Inventory.Infrastructure.Persistence.Configurations;

namespace Inventory.Infrastructure.Persistence;

public sealed class InventoryDbContext : DbContext
{
    public InventoryDbContext(
        DbContextOptions<InventoryDbContext> options)
        : base(options)
    {
    }

    public DbSet<InventoryCategory> InventoryCategories =>
        Set<InventoryCategory>();

    public DbSet<UnitOfMeasure> UnitsOfMeasure =>
        Set<UnitOfMeasure>();

    public DbSet<InventoryItem> InventoryItems =>
        Set<InventoryItem>();

    public DbSet<InventoryStockBalance> InventoryStockBalances => 
        Set<InventoryStockBalance>();

    public DbSet<InventoryStockTransaction> InventoryStockTransactions
    => Set<InventoryStockTransaction>();

    public DbSet<Warehouse> Warehouses => Set<Warehouse>();

    public DbSet<StorageLocation> StorageLocations
    => Set<StorageLocation>();

    public DbSet<InventoryLocationStockBalance> InventoryLocationStockBalances
    => Set<InventoryLocationStockBalance>();
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfiguration(
            new InventoryCategoryConfiguration());

        modelBuilder.ApplyConfiguration(
            new UnitOfMeasureConfiguration());

        modelBuilder.ApplyConfiguration(
            new InventoryItemConfiguration());

        modelBuilder.ApplyConfiguration(
            new InventoryLocationStockBalanceConfiguration());

        modelBuilder.ApplyConfiguration(
            new InventoryStockBalanceConfiguration());

        modelBuilder.ApplyConfiguration(
            new WarehouseConfiguration());

        modelBuilder.ApplyConfiguration(
             new InventoryStockTransactionConfiguration());

        modelBuilder.ApplyConfiguration(
            new StorageLocationConfiguration());


    }
}
