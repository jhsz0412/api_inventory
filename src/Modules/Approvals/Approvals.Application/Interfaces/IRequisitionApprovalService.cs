using Approvals.Application.DTOs.Approvals;

namespace Approvals.Application.Interfaces;

public interface IRequisitionApprovalService
{
    Task<RequisitionApprovalResponse> StartAsync(
        Guid requisitionId,
        Guid departmentId,
        Guid actorEmployeeId,
        CancellationToken cancellationToken = default);

    Task<RequisitionApprovalResponse?> GetByRequisitionIdAsync(
        Guid requisitionId,
        CancellationToken cancellationToken = default);

    Task<RequisitionApprovalResponse> ApproveAsync(
        Guid requisitionId,
        Guid actorEmployeeId,
        ApprovalActionRequest request,
        CancellationToken cancellationToken = default);

    Task<RequisitionApprovalResponse> RejectAsync(
        Guid requisitionId,
        Guid actorEmployeeId,
        ApprovalActionRequest request,
        CancellationToken cancellationToken = default);
}