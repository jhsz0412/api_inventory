using Approvals.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Approvals.Infrastructure.Persistence.Configurations;

public sealed class RequisitionApprovalStepConfiguration
    : IEntityTypeConfiguration<RequisitionApprovalStep>
{
    public void Configure(EntityTypeBuilder<RequisitionApprovalStep> builder)
    {
        builder.ToTable("RequisitionApprovalSteps");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.ApproverEmployeeId).IsRequired();
        builder.Property(x => x.SequenceOrder).IsRequired();
        builder.Property(x => x.Status).IsRequired();
        builder.Property(x => x.ActedAtUtc);
        builder.Property(x => x.Comments).HasMaxLength(2000);

        builder.HasIndex(x => new
        {
            x.RequisitionApprovalId,
            x.SequenceOrder
        }).IsUnique();
    }
}
