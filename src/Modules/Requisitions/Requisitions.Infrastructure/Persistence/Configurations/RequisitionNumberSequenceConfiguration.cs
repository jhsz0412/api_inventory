using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Requisitions.Domain.Entities;

namespace Requisitions.Infrastructure.Persistence.Configurations;

public sealed class RequisitionNumberSequenceConfiguration
    : IEntityTypeConfiguration<RequisitionNumberSequence>
{
    public void Configure(
        EntityTypeBuilder<RequisitionNumberSequence> builder)
    {
        builder.ToTable(
            "RequisitionNumberSequences",
            table => table.HasCheckConstraint(
                "CK_RequisitionNumberSequences_Month",
                "[Month] >= 1 AND [Month] <= 12"));

        builder.HasKey(x => new { x.Year, x.Month });

        builder.Property(x => x.Year)
            .IsRequired();

        builder.Property(x => x.Month)
            .IsRequired();

        builder.Property(x => x.LastNumber)
            .IsRequired();

        builder.ToTable(
            table => table.HasCheckConstraint(
                "CK_RequisitionNumberSequences_LastNumber",
                "[LastNumber] >= 0"));
    }
}