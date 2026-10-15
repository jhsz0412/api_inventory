
using DeliveryEntity = Delivery.Domain.Entities.Delivery;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Delivery.Infrastructure.Persistence.Configurations;

public sealed class DeliveryConfiguration : IEntityTypeConfiguration<DeliveryEntity>
{
    public void Configure(EntityTypeBuilder<DeliveryEntity> builder)
    {
        builder.ToTable("Deliveries");
        builder.HasKey(x => x.Id);

        builder.Property(x => x.DeliveryNumber)
            .HasMaxLength(50)
            .IsRequired();

        builder.HasIndex(x => x.DeliveryNumber).IsUnique();
        builder.Property(x => x.RequisitionId).IsRequired();
        builder.Property(x => x.Status).HasConversion<int>().IsRequired();
        builder.Property(x => x.Remarks).HasMaxLength(500);
        builder.Property(x => x.CreatedAtUtc).IsRequired();
        builder.HasIndex(x => x.RequisitionId);

        builder.HasMany(x => x.Items)
            .WithOne()
            .HasForeignKey(x => x.DeliveryId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Navigation(x => x.Items)
            .UsePropertyAccessMode(PropertyAccessMode.Field);
    }
}
