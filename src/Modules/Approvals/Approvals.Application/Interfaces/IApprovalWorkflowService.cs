using Approvals.Application.DTOs.Workflows;

namespace Approvals.Application.Interfaces;

public interface IApprovalWorkflowService
{
    Task<ApprovalWorkflowResponse> CreateAsync(
        CreateApprovalWorkflowRequest request,
        Guid actorEmployeeId,
        CancellationToken cancellationToken = default);

    Task<ApprovalWorkflowResponse?> GetByIdAsync(
        Guid workflowId,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyCollection<ApprovalWorkflowResponse>> GetByDepartmentAsync(
        Guid departmentId,
        CancellationToken cancellationToken = default);

    Task<ApprovalWorkflowResponse> UpdateAsync(
        Guid workflowId,
        UpdateApprovalWorkflowRequest request,
        Guid actorEmployeeId,
        CancellationToken cancellationToken = default);

    Task<ApprovalWorkflowResponse> SetStatusAsync(
        Guid workflowId,
        SetApprovalWorkflowStatusRequest request,
        Guid actorEmployeeId,
        CancellationToken cancellationToken = default);
}
