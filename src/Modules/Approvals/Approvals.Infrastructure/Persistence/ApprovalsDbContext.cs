using Approvals.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Approvals.Infrastructure.Persistence;

public sealed class ApprovalsDbContext : DbContext
{
    public ApprovalsDbContext(
        DbContextOptions<ApprovalsDbContext> options)
        : base(options)
    {
    }

    public DbSet<DepartmentApprovalWorkflow> DepartmentApprovalWorkflows =>
        Set<DepartmentApprovalWorkflow>();

    public DbSet<DepartmentApprovalWorkflowStep> DepartmentApprovalWorkflowSteps =>
        Set<DepartmentApprovalWorkflowStep>();

    public DbSet<RequisitionApproval> RequisitionApprovals =>
        Set<RequisitionApproval>();

    public DbSet<RequisitionApprovalStep> RequisitionApprovalSteps =>
        Set<RequisitionApprovalStep>();

    public DbSet<ApprovalAuditLog> ApprovalAuditLogs =>
        Set<ApprovalAuditLog>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(
            typeof(ApprovalsDbContext).Assembly);
    }
}
