using Approvals.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Approvals.Infrastructure.Persistence.Configurations;

public sealed class DepartmentApprovalWorkflowConfiguration
    : IEntityTypeConfiguration<DepartmentApprovalWorkflow>
{
    public void Configure(EntityTypeBuilder<DepartmentApprovalWorkflow> builder)
    {
        builder.ToTable("DepartmentApprovalWorkflows");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Name)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(x => x.IsActive).IsRequired();
        builder.Property(x => x.CreatedAtUtc).IsRequired();
        builder.Property(x => x.UpdatedAtUtc).IsRequired();

        builder.HasIndex(x => new { x.DepartmentId, x.IsActive });

        builder.HasMany(x => x.Steps)
            .WithOne()
            .HasForeignKey(x => x.DepartmentApprovalWorkflowId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Navigation(x => x.Steps)
            .HasField("_steps")
            .UsePropertyAccessMode(PropertyAccessMode.Field);
    }
}