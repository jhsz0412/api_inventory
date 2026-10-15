
using Inventory.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Inventory.Infrastructure.Persistence.Configurations;

public sealed class InventoryItemConfiguration
    : IEntityTypeConfiguration<InventoryItem>
{
    public void Configure(
        EntityTypeBuilder<InventoryItem> builder)
    {
        builder.ToTable(
            "InventoryItems",
            table =>
            {
                table.HasCheckConstraint(
                    "CK_InventoryItems_ReorderLevel",
                    "[ReorderLevel] >= 0");
            });

        builder.HasKey(x => x.Id);

        builder.Property(x => x.ItemCode)
            .HasMaxLength(50)
            .IsRequired();

        builder.HasIndex(x => x.ItemCode)
            .IsUnique();

        builder.Property(x => x.Name)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(x => x.Description)
            .HasMaxLength(1000);

        builder.Property(x => x.CategoryId)
            .IsRequired();

        builder.Property(x => x.UnitOfMeasureId)
            .IsRequired();

        builder.Property(x => x.ReorderLevel)
            .HasPrecision(18, 4)
            .IsRequired();

        builder.Property(x => x.IsActive)
            .IsRequired();

        builder.HasOne<InventoryCategory>()
            .WithMany()
            .HasForeignKey(x => x.CategoryId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<UnitOfMeasure>()
            .WithMany()
            .HasForeignKey(x => x.UnitOfMeasureId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
