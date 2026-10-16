using Approvals.Application.DTOs.Audit;

namespace Approvals.Application.Interfaces;

public interface IApprovalAuditService
{
    Task<IReadOnlyCollection<ApprovalAuditLogResponse>> GetByRequisitionIdAsync(
        Guid requisitionId,
        CancellationToken cancellationToken = default);
}