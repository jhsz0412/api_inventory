using Approvals.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Approvals.Infrastructure.Persistence.Configurations;

public sealed class DepartmentApprovalWorkflowStepConfiguration
    : IEntityTypeConfiguration<DepartmentApprovalWorkflowStep>
{
    public void Configure(EntityTypeBuilder<DepartmentApprovalWorkflowStep> builder)
    {
        builder.ToTable("DepartmentApprovalWorkflowSteps");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.ApproverEmployeeId).IsRequired();
        builder.Property(x => x.SequenceOrder).IsRequired();

        builder.HasIndex(x => new
        {
            x.DepartmentApprovalWorkflowId,
            x.SequenceOrder
        }).IsUnique();

        builder.HasIndex(x => new
        {
            x.DepartmentApprovalWorkflowId,
            x.ApproverEmployeeId
        }).IsUnique();
    }
}
