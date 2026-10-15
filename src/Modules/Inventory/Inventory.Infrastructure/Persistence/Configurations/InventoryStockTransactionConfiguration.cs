using Inventory.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Inventory.Infrastructure.Persistence.Configurations;

public sealed class InventoryStockTransactionConfiguration
    : IEntityTypeConfiguration<InventoryStockTransaction>
{
    public void Configure(
        EntityTypeBuilder<InventoryStockTransaction> builder)
    {
        builder.ToTable(
            "InventoryStockTransactions",
            table =>
            {
                table.HasCheckConstraint(
                    "CK_InventoryStockTransactions_Quantity",
                    "[Quantity] > 0");

                table.HasCheckConstraint(
                    "CK_InventoryStockTransactions_QuantityBefore",
                    "[QuantityBefore] >= 0");

                table.HasCheckConstraint(
                    "CK_InventoryStockTransactions_QuantityAfter",
                    "[QuantityAfter] >= 0");
            });

        builder.HasKey(x => x.Id);

        builder.Property(x => x.TransactionType)
            .HasConversion<int>()
            .IsRequired();

        builder.Property(x => x.Quantity)
            .HasPrecision(18, 4)
            .IsRequired();

        builder.Property(x => x.QuantityBefore)
            .HasPrecision(18, 4)
            .IsRequired();

        builder.Property(x => x.QuantityAfter)
            .HasPrecision(18, 4)
            .IsRequired();

        builder.Property(x => x.ReferenceNumber)
            .HasMaxLength(100);

        builder.Property(x => x.Remarks)
            .HasMaxLength(500);

        builder.Property(x => x.CreatedAtUtc)
            .IsRequired();

        builder.HasIndex(x => new
        {
            x.InventoryItemId,
            x.CreatedAtUtc
        });

        builder.HasOne<InventoryItem>()
            .WithMany()
            .HasForeignKey(x => x.InventoryItemId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(x => new
        {
            x.ReferenceNumber,
            x.InventoryItemId,
            x.TransactionType
        })
            .IsUnique()
            .HasFilter(
                "[TransactionType] = 2 AND [ReferenceNumber] IS NOT NULL");

        builder.HasOne<StorageLocation>()
        .WithMany()
        .HasForeignKey(x => x.StorageLocationId)
        .OnDelete(DeleteBehavior.Restrict);
    }
}