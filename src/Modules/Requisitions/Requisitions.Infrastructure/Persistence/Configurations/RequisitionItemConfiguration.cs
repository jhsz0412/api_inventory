using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Requisitions.Domain.Entities;

namespace Requisitions.Infrastructure.Persistence.Configurations;

public sealed class RequisitionItemConfiguration
    : IEntityTypeConfiguration<RequisitionItem>
{
    public void Configure(EntityTypeBuilder<RequisitionItem> builder)
    {
        builder.ToTable(
            "RequisitionItems",
            table => table.HasCheckConstraint(
                "CK_RequisitionItems_QuantityRequested",
                "[QuantityRequested] > 0"));

        builder.HasKey(x => x.Id);

        builder.Property(x => x.RequisitionId)
            .IsRequired();

        builder.Property(x => x.InventoryItemId)
            .IsRequired();

        builder.Property(x => x.QuantityRequested)
            .IsRequired();

        builder.Property(x => x.Notes)
            .HasMaxLength(500);

        builder.HasIndex(x => new
        {
            x.RequisitionId,
            x.InventoryItemId
        }).IsUnique();
    }
}