using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Requisitions.Domain.Entities;

namespace Requisitions.Infrastructure.Persistence.Configurations;

public sealed class RequisitionConfiguration
    : IEntityTypeConfiguration<Requisition>
{
    public void Configure(EntityTypeBuilder<Requisition> builder)
    {
        builder.ToTable("Requisitions");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.RequisitionNumber)
            .HasMaxLength(30)
            .IsRequired();

        builder.HasIndex(x => x.RequisitionNumber)
            .IsUnique();

        builder.Property(x => x.RequestedByEmployeeId)
            .IsRequired();

        builder.Property(x => x.DepartmentId)
            .IsRequired();

        builder.Property(x => x.FiscalYearId)
            .IsRequired();

        builder.Property(x => x.RequestedAtUtc)
            .IsRequired();

        builder.Property(x => x.Purpose)
            .HasMaxLength(500)
            .IsRequired();

        builder.Property(x => x.Status)
            .HasConversion<string>()
            .HasMaxLength(30)
            .IsRequired();

        builder.HasMany(x => x.Items)
            .WithOne()
            .HasForeignKey(x => x.RequisitionId)
            .OnDelete(DeleteBehavior.Cascade);

                builder.Navigation(x => x.Items)
                    .UsePropertyAccessMode(PropertyAccessMode.Field);

        builder.Property(x => x.RowVersion)
            .IsRowVersion();
    }
}