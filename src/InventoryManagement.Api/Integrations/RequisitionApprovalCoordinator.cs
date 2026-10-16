
using Approvals.Application.DTOs.Approvals;
using Approvals.Application.Interfaces;
using Requisitions.Application.Interfaces;

namespace InventoryManagement.Api.Integrations;

public sealed class RequisitionApprovalCoordinator(
    IRequisitionService requisitionService,
    IRequisitionApprovalService approvalService,
    IApprovalWorkflowService workflowService)
{
    public async Task<object?> SubmitAsync(
        Guid requisitionId,
        Guid actorEmployeeId,
        CancellationToken cancellationToken = default)
    {
        var existingRequisition = await requisitionService.GetByIdAsync(
            requisitionId,
            cancellationToken);

        if (existingRequisition is null)
            return null;

        var workflows = await workflowService.GetByDepartmentAsync(
            existingRequisition.DepartmentId,
            cancellationToken);

        if (!workflows.Any(workflow => workflow.IsActive))
        {
            throw new InvalidOperationException(
                "No active approval workflow is configured for this department.");
        }

        var requisition = await requisitionService.SubmitAsync(
            requisitionId,
            cancellationToken);

        if (requisition is null)
            return null;

        var approval = await approvalService.StartAsync(
            requisition.Id,
            requisition.DepartmentId,
            actorEmployeeId,
            cancellationToken);

        return new { requisition, approval };
    }

    public async Task<object?> ApproveAsync(
        Guid requisitionId,
        Guid actorEmployeeId,
        ApprovalActionRequest request,
        CancellationToken cancellationToken = default)
        {
        var requisition = await requisitionService.GetByIdAsync(
            requisitionId,
            cancellationToken);

        if (requisition is null)
            return null;

        if (requisition.Status != "Submitted")
            throw new InvalidOperationException(
                "Only submitted requisitions can be approved.");

        var approval = await approvalService.ApproveAsync(
            requisitionId,
            actorEmployeeId,
            request,
            cancellationToken);

        if (!approval.IsCompleted)
            return new { approval };

        var updatedRequisition = await requisitionService.ApproveAsync(
            requisitionId,
            cancellationToken);

        return new
        {
            approval,
            requisition = updatedRequisition
        };
    }

    public async Task<object?> RejectAsync(
        Guid requisitionId,
        Guid actorEmployeeId,
        ApprovalActionRequest request,
        CancellationToken cancellationToken = default)
    {
        var requisition = await requisitionService.GetByIdAsync(
            requisitionId,
            cancellationToken);

        if (requisition is null)
            return null;

        if (requisition.Status != "Submitted")
            throw new InvalidOperationException(
                "Only submitted requisitions can be rejected.");

        var approval = await approvalService.RejectAsync(
            requisitionId,
            actorEmployeeId,
            request,
            cancellationToken);

        var updatedRequisition = await requisitionService.RejectAsync(
            requisitionId,
            cancellationToken);

        return new
        {
            approval,
            requisition = updatedRequisition
        };
    }
}
