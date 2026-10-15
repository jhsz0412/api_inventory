using Inventory.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Inventory.Infrastructure.Persistence.Configurations;

public sealed class InventoryStockBalanceConfiguration
    : IEntityTypeConfiguration<InventoryStockBalance>
{
    public void Configure(EntityTypeBuilder<InventoryStockBalance> builder)
    {
        builder.ToTable(
            "InventoryStockBalances",
            table => table.HasCheckConstraint(
                "CK_InventoryStockBalances_QuantityOnHand",
                "[QuantityOnHand] >= 0"));

        builder.HasKey(x => x.Id);

        builder.Property(x => x.QuantityOnHand)
            .HasPrecision(18, 4)
            .IsRequired();

        builder.Property(x => x.LastUpdatedUtc)
            .IsRequired();

        builder.HasIndex(x => x.InventoryItemId)
            .IsUnique();

        builder.HasOne<InventoryItem>()
            .WithMany()
            .HasForeignKey(x => x.InventoryItemId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Property(x => x.RowVersion)
            .IsRowVersion();
    }
}