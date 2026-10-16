using Approvals.Domain.Enums;

namespace Approvals.Domain.Entities;

public sealed class ApprovalAuditLog
{
    public Guid Id { get; private set; }
    public Guid? RequisitionApprovalId { get; private set; }
    public Guid? RequisitionId { get; private set; }
    public Guid? DepartmentId { get; private set; }
    public Guid ActorEmployeeId { get; private set; }
    public ApprovalAuditAction Action { get; private set; }
    public int? SequenceOrder { get; private set; }
    public string? PreviousStatus { get; private set; }
    public string? NewStatus { get; private set; }
    public string? Comments { get; private set; }
    public DateTime OccurredAtUtc { get; private set; }

    private ApprovalAuditLog()
    {
    }

    public ApprovalAuditLog(
        Guid actorEmployeeId,
        ApprovalAuditAction action,
        Guid? requisitionApprovalId = null,
        Guid? requisitionId = null,
        Guid? departmentId = null,
        int? sequenceOrder = null,
        string? previousStatus = null,
        string? newStatus = null,
        string? comments = null)
    {
        if (actorEmployeeId == Guid.Empty)
            throw new ArgumentException(
                "Actor employee ID is required.",
                nameof(actorEmployeeId));

        if (requisitionApprovalId == Guid.Empty)
            throw new ArgumentException(
                "Approval ID cannot be empty.",
                nameof(requisitionApprovalId));

        if (requisitionId == Guid.Empty)
            throw new ArgumentException(
                "Requisition ID cannot be empty.",
                nameof(requisitionId));

        if (departmentId == Guid.Empty)
            throw new ArgumentException(
                "Department ID cannot be empty.",
                nameof(departmentId));

        if (sequenceOrder is < 1)
            throw new ArgumentOutOfRangeException(
                nameof(sequenceOrder));

        Id = Guid.NewGuid();
        ActorEmployeeId = actorEmployeeId;
        Action = action;
        RequisitionApprovalId = requisitionApprovalId;
        RequisitionId = requisitionId;
        DepartmentId = departmentId;
        SequenceOrder = sequenceOrder;
        PreviousStatus = Normalize(previousStatus, 100);
        NewStatus = Normalize(newStatus, 100);
        Comments = Normalize(comments, 2000);
        OccurredAtUtc = DateTime.UtcNow;
    }

    private static string? Normalize(string? value, int maxLength)
    {
        if (string.IsNullOrWhiteSpace(value))
            return null;

        var normalized = value.Trim();

        if (normalized.Length > maxLength)
            throw new ArgumentException(
                $"Value cannot exceed {maxLength} characters.");

        return normalized;
    }
}
