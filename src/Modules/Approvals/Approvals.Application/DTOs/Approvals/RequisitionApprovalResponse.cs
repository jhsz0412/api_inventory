using Approvals.Domain.Enums;

namespace Approvals.Application.DTOs.Approvals;

public sealed class RequisitionApprovalResponse
{
    public Guid Id { get; init; }

    public Guid RequisitionId { get; init; }

    public Guid DepartmentId { get; init; }

    public bool IsCompleted { get; init; }

    public bool IsRejected { get; init; }

    public DateTime StartedAtUtc { get; init; }

    public DateTime? CompletedAtUtc { get; init; }

    public Guid? CurrentApproverEmployeeId { get; init; }

    public List<RequisitionApprovalStepResponse> Steps { get; init; } = [];
}

public sealed class RequisitionApprovalStepResponse
{
    public Guid Id { get; init; }

    public Guid ApproverEmployeeId { get; init; }

    public int SequenceOrder { get; init; }

    public ApprovalStepStatus Status { get; init; }

    public DateTime? ActedAtUtc { get; init; }

    public string? Comments { get; init; }
}