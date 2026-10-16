namespace Approvals.Application.DTOs.Workflows;

public sealed class CreateApprovalWorkflowRequest
{
    public Guid DepartmentId { get; init; }

    public string Name { get; init; } = string.Empty;

    public List<ApprovalWorkflowStepRequest> Steps { get; init; } = [];
}

public sealed class ApprovalWorkflowStepRequest
{
    public Guid ApproverEmployeeId { get; init; }

    public int SequenceOrder { get; init; }
}