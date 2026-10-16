using Approvals.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Approvals.Infrastructure.Persistence.Configurations;

public sealed class ApprovalAuditLogConfiguration
    : IEntityTypeConfiguration<ApprovalAuditLog>
{
    public void Configure(EntityTypeBuilder<ApprovalAuditLog> builder)
    {
        builder.ToTable("ApprovalAuditLogs");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.ActorEmployeeId).IsRequired();
        builder.Property(x => x.Action).IsRequired();
        builder.Property(x => x.SequenceOrder);
        builder.Property(x => x.PreviousStatus).HasMaxLength(100);
        builder.Property(x => x.NewStatus).HasMaxLength(100);
        builder.Property(x => x.Comments).HasMaxLength(2000);
        builder.Property(x => x.OccurredAtUtc).IsRequired();

        builder.HasIndex(x => new { x.RequisitionId, x.OccurredAtUtc });
        builder.HasIndex(x => new { x.DepartmentId, x.OccurredAtUtc });
        builder.HasIndex(x => new { x.ActorEmployeeId, x.OccurredAtUtc });
    }
}
