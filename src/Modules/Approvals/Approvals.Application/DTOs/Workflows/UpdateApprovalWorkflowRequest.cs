namespace Approvals.Application.DTOs.Workflows;

public sealed class UpdateApprovalWorkflowRequest
{
    public string Name { get; init; } = string.Empty;

    public List<ApprovalWorkflowStepRequest> Steps { get; init; } = [];
}