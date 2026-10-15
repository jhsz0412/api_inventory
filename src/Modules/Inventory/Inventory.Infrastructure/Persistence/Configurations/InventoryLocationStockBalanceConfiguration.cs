using Inventory.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Inventory.Infrastructure.Persistence.Configurations;

public sealed class InventoryLocationStockBalanceConfiguration
    : IEntityTypeConfiguration<InventoryLocationStockBalance>
{
    public void Configure(
        EntityTypeBuilder<InventoryLocationStockBalance> builder)
    {
        builder.ToTable(
            "InventoryLocationStockBalances",
            table => table.HasCheckConstraint(
                "CK_InventoryLocationStockBalances_QuantityOnHand",
                "[QuantityOnHand] >= 0"));

        builder.HasKey(x => x.Id);

        builder.Property(x => x.QuantityOnHand)
            .HasPrecision(18, 4)
            .IsRequired();

        builder.Property(x => x.LastUpdatedUtc)
            .IsRequired();

        builder.HasIndex(x => new
        {
            x.InventoryItemId,
            x.StorageLocationId
        })
            .IsUnique();

        builder.HasOne<InventoryItem>()
            .WithMany()
            .HasForeignKey(x => x.InventoryItemId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<StorageLocation>()
            .WithMany()
            .HasForeignKey(x => x.StorageLocationId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Property(x => x.RowVersion)
            .IsRowVersion();
    }
}