using Approvals.Domain.Enums;

namespace Approvals.Application.DTOs.Audit;

public sealed class ApprovalAuditLogResponse
{
    public Guid Id { get; init; }

    public Guid? RequisitionId { get; init; }

    public Guid ActorEmployeeId { get; init; }

    public ApprovalAuditAction Action { get; init; }

    public int? SequenceOrder { get; init; }

    public string? PreviousStatus { get; init; }

    public string? NewStatus { get; init; }

    public string? Comments { get; init; }

    public DateTime OccurredAtUtc { get; init; }
}
