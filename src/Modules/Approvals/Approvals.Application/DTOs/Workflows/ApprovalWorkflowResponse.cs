namespace Approvals.Application.DTOs.Workflows;

public sealed class ApprovalWorkflowResponse
{
    public Guid Id { get; init; }

    public Guid DepartmentId { get; init; }

    public string Name { get; init; } = string.Empty;

    public bool IsActive { get; init; }

    public DateTime CreatedAtUtc { get; init; }

    public DateTime UpdatedAtUtc { get; init; }

    public List<ApprovalWorkflowStepResponse> Steps { get; init; } = [];
}

public sealed class ApprovalWorkflowStepResponse
{
    public Guid Id { get; init; }

    public Guid ApproverEmployeeId { get; init; }

    public int SequenceOrder { get; init; }
}