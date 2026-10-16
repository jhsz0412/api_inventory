using Approvals.Domain.Entities;

namespace Approvals.Application.Interfaces;

public interface IApprovalsRepository
{
    Task<DepartmentApprovalWorkflow?> GetWorkflowByIdAsync(
        Guid workflowId,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyCollection<DepartmentApprovalWorkflow>> GetWorkflowsByDepartmentAsync(
        Guid departmentId,
        CancellationToken cancellationToken = default);

    Task<DepartmentApprovalWorkflow?> GetActiveWorkflowByDepartmentAsync(
        Guid departmentId,
        CancellationToken cancellationToken = default);

    Task<RequisitionApproval?> GetApprovalByRequisitionIdAsync(
        Guid requisitionId,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyCollection<ApprovalAuditLog>> GetAuditLogsByRequisitionIdAsync(
        Guid requisitionId,
        CancellationToken cancellationToken = default);

    void AddWorkflow(DepartmentApprovalWorkflow workflow);

    void AddApproval(RequisitionApproval approval);

    void AddAuditLog(ApprovalAuditLog auditLog);

    Task SaveChangesAsync(
        CancellationToken cancellationToken = default);
}