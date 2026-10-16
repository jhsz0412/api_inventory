using Approvals.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Approvals.Infrastructure.Persistence.Configurations;

public sealed class RequisitionApprovalConfiguration
    : IEntityTypeConfiguration<RequisitionApproval>
{
    public void Configure(EntityTypeBuilder<RequisitionApproval> builder)
    {
        builder.ToTable("RequisitionApprovals");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.RequisitionId).IsRequired();
        builder.Property(x => x.DepartmentId).IsRequired();
        builder.Property(x => x.IsCompleted).IsRequired();
        builder.Property(x => x.IsRejected).IsRequired();
        builder.Property(x => x.StartedAtUtc).IsRequired();

        builder.HasIndex(x => x.RequisitionId).IsUnique();

        builder.HasMany(x => x.Steps)
            .WithOne()
            .HasForeignKey(x => x.RequisitionApprovalId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Navigation(x => x.Steps)
            .HasField("_steps")
            .UsePropertyAccessMode(PropertyAccessMode.Field);
    }
}
