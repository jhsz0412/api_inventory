using Approvals.Application.Interfaces;
using Approvals.Domain.Entities;
using Approvals.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Approvals.Infrastructure.Persistence.Repositories;

public sealed class ApprovalsRepository : IApprovalsRepository
{
    private readonly ApprovalsDbContext _context;

    public ApprovalsRepository(ApprovalsDbContext context)
    {
        _context = context;
    }

    public async Task<DepartmentApprovalWorkflow?> GetWorkflowByIdAsync(
        Guid workflowId,
        CancellationToken cancellationToken = default)
    {
        return await _context.DepartmentApprovalWorkflows
            .Include(x => x.Steps)
            .FirstOrDefaultAsync(
                x => x.Id == workflowId,
                cancellationToken);
    }

    public async Task<IReadOnlyCollection<DepartmentApprovalWorkflow>>
        GetWorkflowsByDepartmentAsync(
            Guid departmentId,
            CancellationToken cancellationToken = default)
    {
        return await _context.DepartmentApprovalWorkflows
            .Include(x => x.Steps)
            .Where(x => x.DepartmentId == departmentId)
            .OrderBy(x => x.Name)
            .ToListAsync(cancellationToken);
    }

    public async Task<DepartmentApprovalWorkflow?>
        GetActiveWorkflowByDepartmentAsync(
            Guid departmentId,
            CancellationToken cancellationToken = default)
    {
        return await _context.DepartmentApprovalWorkflows
            .Include(x => x.Steps)
            .FirstOrDefaultAsync(
                x => x.DepartmentId == departmentId && x.IsActive,
                cancellationToken);
    }

    public async Task<RequisitionApproval?> GetApprovalByRequisitionIdAsync(
        Guid requisitionId,
        CancellationToken cancellationToken = default)
    {
        return await _context.RequisitionApprovals
            .Include(x => x.Steps)
            .FirstOrDefaultAsync(
                x => x.RequisitionId == requisitionId,
                cancellationToken);
    }

    public async Task<IReadOnlyCollection<ApprovalAuditLog>>
        GetAuditLogsByRequisitionIdAsync(
            Guid requisitionId,
            CancellationToken cancellationToken = default)
    {
        return await _context.ApprovalAuditLogs
            .Where(x => x.RequisitionId == requisitionId)
            .OrderBy(x => x.OccurredAtUtc)
            .ToListAsync(cancellationToken);
    }

    public void AddWorkflow(DepartmentApprovalWorkflow workflow)
    {
        _context.DepartmentApprovalWorkflows.Add(workflow);
    }

    public void AddApproval(RequisitionApproval approval)
    {
        _context.RequisitionApprovals.Add(approval);
    }

    public void AddAuditLog(ApprovalAuditLog auditLog)
    {
        _context.ApprovalAuditLogs.Add(auditLog);
    }

    public async Task SaveChangesAsync(
        CancellationToken cancellationToken = default)
    {
        await _context.SaveChangesAsync(cancellationToken);
    }
}