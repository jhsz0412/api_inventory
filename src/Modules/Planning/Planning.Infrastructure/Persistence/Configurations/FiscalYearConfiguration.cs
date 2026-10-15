
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Planning.Domain.Entities;

namespace Planning.Infrastructure.Persistence.Configurations;

public sealed class FiscalYearConfiguration
    : IEntityTypeConfiguration<FiscalYear>
{
    public void Configure(EntityTypeBuilder<FiscalYear> builder)
    {
        builder.ToTable("FiscalYears");

        builder.HasKey(fiscalYear => fiscalYear.Id);

        builder.Property(fiscalYear => fiscalYear.Name)
            .HasMaxLength(100)
            .IsRequired();

        builder.HasIndex(fiscalYear => fiscalYear.Name)
            .IsUnique();

        builder.Property(fiscalYear => fiscalYear.StartDate)
            .IsRequired();

        builder.Property(fiscalYear => fiscalYear.EndDate)
            .IsRequired();

        builder.Property(fiscalYear => fiscalYear.IsActive)
            .IsRequired();

        builder.Property(fiscalYear => fiscalYear.CreatedAtUtc)
            .IsRequired();

        builder.ToTable(table => table.HasCheckConstraint(
            "CK_FiscalYears_DateRange",
            "[EndDate] >= [StartDate]"));
    }
}
