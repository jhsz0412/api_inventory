using Approvals.Application.DTOs.Approvals;
using Approvals.Application.Interfaces;
using Approvals.Domain.Entities;
using Approvals.Domain.Enums;

namespace Approvals.Application.Services;

public sealed class RequisitionApprovalService
    : IRequisitionApprovalService
{
    private readonly IApprovalsRepository _repository;

    public RequisitionApprovalService(IApprovalsRepository repository)
    {
        _repository = repository;
    }

    public async Task<RequisitionApprovalResponse> StartAsync(
        Guid requisitionId,
        Guid departmentId,
        Guid actorEmployeeId,
        CancellationToken cancellationToken = default)
    {
        EnsureRequiredIds(requisitionId, departmentId, actorEmployeeId);

        var existing = await _repository.GetApprovalByRequisitionIdAsync(
            requisitionId, cancellationToken);

        if (existing is not null)
            throw new InvalidOperationException(
                "Approval has already been started for this requisition.");

        var workflow = await _repository.GetActiveWorkflowByDepartmentAsync(
            departmentId, cancellationToken);

        if (workflow is null)
            throw new InvalidOperationException(
                "The department has no active approval workflow.");

        var approvers = workflow.Steps
            .OrderBy(x => x.SequenceOrder)
            .Select(x => (x.ApproverEmployeeId, x.SequenceOrder))
            .ToList();

        var approval = new RequisitionApproval(
            requisitionId, departmentId, approvers);

        _repository.AddApproval(approval);

        _repository.AddAuditLog(new ApprovalAuditLog(
            actorEmployeeId,
            ApprovalAuditAction.ApprovalStarted,
            requisitionApprovalId: approval.Id,
            requisitionId: requisitionId,
            departmentId: departmentId,
            previousStatus: "Submitted",
            newStatus: "PendingApproval"));

        await _repository.SaveChangesAsync(cancellationToken);

        return Map(approval);
    }

    public async Task<RequisitionApprovalResponse?> GetByRequisitionIdAsync(
        Guid requisitionId,
        CancellationToken cancellationToken = default)
    {
        var approval = await _repository.GetApprovalByRequisitionIdAsync(
            requisitionId, cancellationToken);

        return approval is null ? null : Map(approval);
    }

    public async Task<RequisitionApprovalResponse> ApproveAsync(
        Guid requisitionId,
        Guid actorEmployeeId,
        ApprovalActionRequest request,
        CancellationToken cancellationToken = default)
    {
        EnsureRequiredIds(requisitionId, actorEmployeeId);

        ArgumentNullException.ThrowIfNull(request);

        var approval = await GetRequiredApprovalAsync(
            requisitionId, cancellationToken);

        var currentStep = approval.GetCurrentStep();
        var sequenceOrder = currentStep.SequenceOrder;

        approval.ApproveCurrentStep(actorEmployeeId, request.Comments);

        _repository.AddAuditLog(new ApprovalAuditLog(
            actorEmployeeId,
            ApprovalAuditAction.StepApproved,
            requisitionApprovalId: approval.Id,
            requisitionId: requisitionId,
            departmentId: approval.DepartmentId,
            sequenceOrder: sequenceOrder,
            previousStatus: "Pending",
            newStatus: "Approved",
            comments: request.Comments));

        if (approval.IsCompleted)
        {
            _repository.AddAuditLog(new ApprovalAuditLog(
                actorEmployeeId,
                ApprovalAuditAction.ApprovalCompleted,
                requisitionApprovalId: approval.Id,
                requisitionId: requisitionId,
                departmentId: approval.DepartmentId,
                previousStatus: "PendingApproval",
                newStatus: "Approved"));
        }

        await _repository.SaveChangesAsync(cancellationToken);

        return Map(approval);
    }

    public async Task<RequisitionApprovalResponse> RejectAsync(
        Guid requisitionId,
        Guid actorEmployeeId,
        ApprovalActionRequest request,
        CancellationToken cancellationToken = default)
    {
        EnsureRequiredIds(requisitionId, actorEmployeeId);

        ArgumentNullException.ThrowIfNull(request);

        var approval = await GetRequiredApprovalAsync(
            requisitionId, cancellationToken);

        var currentStep = approval.GetCurrentStep();
        var sequenceOrder = currentStep.SequenceOrder;

        approval.RejectCurrentStep(actorEmployeeId, request.Comments);

        _repository.AddAuditLog(new ApprovalAuditLog(
            actorEmployeeId,
            ApprovalAuditAction.StepRejected,
            requisitionApprovalId: approval.Id,
            requisitionId: requisitionId,
            departmentId: approval.DepartmentId,
            sequenceOrder: sequenceOrder,
            previousStatus: "Pending",
            newStatus: "Rejected",
            comments: request.Comments));

        await _repository.SaveChangesAsync(cancellationToken);

        return Map(approval);
    }

    private async Task<RequisitionApproval> GetRequiredApprovalAsync(
        Guid requisitionId,
        CancellationToken cancellationToken)
    {
        return await _repository.GetApprovalByRequisitionIdAsync(
            requisitionId, cancellationToken)
            ?? throw new KeyNotFoundException(
                "Approval has not been started for this requisition.");
    }

    private static void EnsureRequiredIds(params Guid[] ids)
    {
        if (ids.Any(id => id == Guid.Empty))
            throw new ArgumentException("Required IDs cannot be empty.");
    }

    private static RequisitionApprovalResponse Map(
        RequisitionApproval approval)
    {
        var currentApprover = !approval.IsCompleted && !approval.IsRejected
            ? approval.Steps
                .Where(x => x.Status == ApprovalStepStatus.Pending)
                .OrderBy(x => x.SequenceOrder)
                .Select(x => (Guid?)x.ApproverEmployeeId)
                .FirstOrDefault()
            : null;

        return new RequisitionApprovalResponse
        {
            Id = approval.Id,
            RequisitionId = approval.RequisitionId,
            DepartmentId = approval.DepartmentId,
            IsCompleted = approval.IsCompleted,
            IsRejected = approval.IsRejected,
            StartedAtUtc = approval.StartedAtUtc,
            CompletedAtUtc = approval.CompletedAtUtc,
            CurrentApproverEmployeeId = currentApprover,
            Steps = approval.Steps
                .OrderBy(x => x.SequenceOrder)
                .Select(x => new RequisitionApprovalStepResponse
                {
                    Id = x.Id,
                    ApproverEmployeeId = x.ApproverEmployeeId,
                    SequenceOrder = x.SequenceOrder,
                    Status = x.Status,
                    ActedAtUtc = x.ActedAtUtc,
                    Comments = x.Comments
                })
                .ToList()
        };
    }
}