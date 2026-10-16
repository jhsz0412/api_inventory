using Approvals.Application.DTOs.Workflows;
using Approvals.Application.Interfaces;
using Approvals.Domain.Entities;

namespace Approvals.Application.Services;

public sealed class ApprovalWorkflowService : IApprovalWorkflowService
{
    private readonly IApprovalsRepository _repository;

    public ApprovalWorkflowService(IApprovalsRepository repository)
    {
        _repository = repository;
    }

    public async Task<ApprovalWorkflowResponse> CreateAsync(
        CreateApprovalWorkflowRequest request,
        Guid actorEmployeeId,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);
        EnsureActor(actorEmployeeId);
        ValidateSteps(request.Steps);

        var existing = await _repository.GetWorkflowsByDepartmentAsync(
            request.DepartmentId, cancellationToken);

        if (existing.Any(x => string.Equals(
            x.Name, request.Name.Trim(), StringComparison.OrdinalIgnoreCase)))
            throw new InvalidOperationException(
                "A workflow with this name already exists for the department.");

        var workflow = new DepartmentApprovalWorkflow(
            request.DepartmentId, request.Name);

        foreach (var step in request.Steps.OrderBy(x => x.SequenceOrder))
            workflow.AddStep(step.ApproverEmployeeId, step.SequenceOrder);

        _repository.AddWorkflow(workflow);
        _repository.AddAuditLog(new ApprovalAuditLog(
            actorEmployeeId,
            Approvals.Domain.Enums.ApprovalAuditAction.WorkflowCreated,
            departmentId: request.DepartmentId,
            newStatus: "Created",
            comments: workflow.Name));

        await _repository.SaveChangesAsync(cancellationToken);

        return Map(workflow);
    }

    public async Task<ApprovalWorkflowResponse?> GetByIdAsync(
        Guid workflowId,
        CancellationToken cancellationToken = default)
    {
        var workflow = await _repository.GetWorkflowByIdAsync(
            workflowId, cancellationToken);

        return workflow is null ? null : Map(workflow);
    }

    public async Task<IReadOnlyCollection<ApprovalWorkflowResponse>>
        GetByDepartmentAsync(
            Guid departmentId,
            CancellationToken cancellationToken = default)
    {
        var workflows = await _repository.GetWorkflowsByDepartmentAsync(
            departmentId, cancellationToken);

        return workflows.Select(Map).ToList();
    }

    public async Task<ApprovalWorkflowResponse> UpdateAsync(
        Guid workflowId,
        UpdateApprovalWorkflowRequest request,
        Guid actorEmployeeId,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);
        EnsureActor(actorEmployeeId);
        ValidateSteps(request.Steps);

        var workflow = await _repository.GetWorkflowByIdAsync(
            workflowId, cancellationToken)
            ?? throw new KeyNotFoundException("Approval workflow not found.");

        if (workflow.IsActive)
            throw new InvalidOperationException(
                "Deactivate the workflow before changing its approvers.");

        var departmentWorkflows = await _repository.GetWorkflowsByDepartmentAsync(
            workflow.DepartmentId, cancellationToken);

        if (departmentWorkflows.Any(x => x.Id != workflowId &&
            string.Equals(x.Name, request.Name.Trim(),
                StringComparison.OrdinalIgnoreCase)))
            throw new InvalidOperationException(
                "A workflow with this name already exists for the department.");

        var replacement = new DepartmentApprovalWorkflow(
            workflow.DepartmentId, request.Name);

        foreach (var step in request.Steps.OrderBy(x => x.SequenceOrder))
            replacement.AddStep(step.ApproverEmployeeId, step.SequenceOrder);

        workflow.ReplaceConfiguration(replacement.Name, replacement.Steps);

        _repository.AddAuditLog(new ApprovalAuditLog(
            actorEmployeeId,
            Approvals.Domain.Enums.ApprovalAuditAction.WorkflowUpdated,
            departmentId: workflow.DepartmentId,
            previousStatus: "Configured",
            newStatus: "Updated",
            comments: workflow.Name));

        await _repository.SaveChangesAsync(cancellationToken);

        return Map(workflow);
    }

    public async Task<ApprovalWorkflowResponse> SetStatusAsync(
        Guid workflowId,
        SetApprovalWorkflowStatusRequest request,
        Guid actorEmployeeId,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);
        EnsureActor(actorEmployeeId);

        var workflow = await _repository.GetWorkflowByIdAsync(
            workflowId, cancellationToken)
            ?? throw new KeyNotFoundException("Approval workflow not found.");

        var previousStatus = workflow.IsActive ? "Active" : "Inactive";

        if (request.IsActive)
        {
            var otherWorkflows = await _repository.GetWorkflowsByDepartmentAsync(
                workflow.DepartmentId, cancellationToken);

            foreach (var other in otherWorkflows.Where(
                x => x.Id != workflow.Id && x.IsActive))
                other.Deactivate();

            workflow.Activate();
        }
        else
        {
            workflow.Deactivate();
        }

        _repository.AddAuditLog(new ApprovalAuditLog(
            actorEmployeeId,
            Approvals.Domain.Enums.ApprovalAuditAction.WorkflowUpdated,
            departmentId: workflow.DepartmentId,
            previousStatus: previousStatus,
            newStatus: workflow.IsActive ? "Active" : "Inactive",
            comments: workflow.Name));

        await _repository.SaveChangesAsync(cancellationToken);

        return Map(workflow);
    }

    private static void ValidateSteps(
        IReadOnlyCollection<ApprovalWorkflowStepRequest>? steps)
    {
        if (steps is null || steps.Count == 0)
            throw new ArgumentException(
                "At least one approver is required.", nameof(steps));

        if (steps.Any(x => x.ApproverEmployeeId == Guid.Empty))
            throw new ArgumentException("Approver employee IDs are required.");

        if (steps.Select(x => x.ApproverEmployeeId).Distinct().Count()
            != steps.Count)
            throw new ArgumentException(
                "An employee cannot appear more than once in a workflow.");

        var ordered = steps.OrderBy(x => x.SequenceOrder).ToList();

        for (var i = 0; i < ordered.Count; i++)
            if (ordered[i].SequenceOrder != i + 1)
                throw new ArgumentException(
                    "Sequence orders must be consecutive, starting at 1.");
    }

    private static void EnsureActor(Guid actorEmployeeId)
    {
        if (actorEmployeeId == Guid.Empty)
            throw new ArgumentException("Actor employee ID is required.");
    }

    private static ApprovalWorkflowResponse Map(
        DepartmentApprovalWorkflow workflow)
    {
        return new ApprovalWorkflowResponse
        {
            Id = workflow.Id,
            DepartmentId = workflow.DepartmentId,
            Name = workflow.Name,
            IsActive = workflow.IsActive,
            CreatedAtUtc = workflow.CreatedAtUtc,
            UpdatedAtUtc = workflow.UpdatedAtUtc,
            Steps = workflow.Steps
                .OrderBy(x => x.SequenceOrder)
                .Select(x => new ApprovalWorkflowStepResponse
                {
                    Id = x.Id,
                    ApproverEmployeeId = x.ApproverEmployeeId,
                    SequenceOrder = x.SequenceOrder
                }).ToList()
        };
    }
}