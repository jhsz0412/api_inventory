
using Delivery.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Delivery.Infrastructure.Persistence.Configurations;

public sealed class DeliveryItemConfiguration
    : IEntityTypeConfiguration<DeliveryItem>
{
    public void Configure(EntityTypeBuilder<DeliveryItem> builder)
    {
        builder.ToTable(
            "DeliveryItems",
            table => table.HasCheckConstraint(
                "CK_DeliveryItems_QuantityPrepared",
                "[QuantityPrepared] > 0"));

        builder.HasKey(x => x.Id);

        builder.Property(x => x.QuantityPrepared)
            .HasPrecision(18, 4)
            .IsRequired();

        builder.HasIndex(x => new
        {
            x.DeliveryId,
            x.InventoryItemId
        })
            .IsUnique();

        builder.Property(x => x.InventoryItemId)
            .IsRequired();

        builder.Property(x => x.StorageLocationId)
            .IsRequired();
    }
}
