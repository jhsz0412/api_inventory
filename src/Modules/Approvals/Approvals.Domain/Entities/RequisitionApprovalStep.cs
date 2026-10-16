using Approvals.Domain.Enums;

namespace Approvals.Domain.Entities;

public sealed class RequisitionApprovalStep
{
    public Guid Id { get; private set; }
    public Guid RequisitionApprovalId { get; private set; }
    public Guid ApproverEmployeeId { get; private set; }
    public int SequenceOrder { get; private set; }
    public ApprovalStepStatus Status { get; private set; }
    public DateTime? ActedAtUtc { get; private set; }
    public string? Comments { get; private set; }

    private RequisitionApprovalStep()
    {
    }

    internal RequisitionApprovalStep(
        Guid approvalId,
        Guid approverEmployeeId,
        int sequenceOrder)
    {
        Id = Guid.NewGuid();
        RequisitionApprovalId = approvalId;
        ApproverEmployeeId = approverEmployeeId;
        SequenceOrder = sequenceOrder;
        Status = ApprovalStepStatus.Pending;
    }

    internal void SetApprovalId(Guid approvalId)
    {
        if (approvalId == Guid.Empty)
            throw new ArgumentException(
                "Approval ID is required.",
                nameof(approvalId));

        RequisitionApprovalId = approvalId;
    }

    internal void Approve(string? comments)
    {
        EnsurePending();

        Status = ApprovalStepStatus.Approved;
        ActedAtUtc = DateTime.UtcNow;
        Comments = NormalizeComments(comments);
    }

    internal void Reject(string? comments)
    {
        EnsurePending();

        Status = ApprovalStepStatus.Rejected;
        ActedAtUtc = DateTime.UtcNow;
        Comments = NormalizeComments(comments);
    }

    private void EnsurePending()
    {
        if (Status != ApprovalStepStatus.Pending)
            throw new InvalidOperationException(
                "Only pending approval steps can receive a decision.");
    }

    private static string? NormalizeComments(string? comments)
    {
        if (string.IsNullOrWhiteSpace(comments))
            return null;

        var normalized = comments.Trim();

        if (normalized.Length > 2000)
            throw new ArgumentException(
                "Comments cannot exceed 2000 characters.",
                nameof(comments));

        return normalized;
    }
}