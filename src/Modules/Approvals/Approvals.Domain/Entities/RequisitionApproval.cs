using Approvals.Domain.Enums;

namespace Approvals.Domain.Entities;

public sealed class RequisitionApproval
{
    public Guid Id { get; private set; }
    public Guid RequisitionId { get; private set; }
    public Guid DepartmentId { get; private set; }
    public bool IsCompleted { get; private set; }
    public bool IsRejected { get; private set; }
    public DateTime StartedAtUtc { get; private set; }
    public DateTime? CompletedAtUtc { get; private set; }

    private readonly List<RequisitionApprovalStep> _steps = [];

    public IReadOnlyCollection<RequisitionApprovalStep> Steps =>
        _steps.AsReadOnly();

    private RequisitionApproval()
    {
    }

    public RequisitionApproval(
        Guid requisitionId,
        Guid departmentId,
        IEnumerable<(Guid ApproverEmployeeId, int SequenceOrder)> approvers)
    {
        if (requisitionId == Guid.Empty)
            throw new ArgumentException(
                "Requisition ID is required.",
                nameof(requisitionId));

        if (departmentId == Guid.Empty)
            throw new ArgumentException(
                "Department ID is required.",
                nameof(departmentId));

        ArgumentNullException.ThrowIfNull(approvers);

        var assignedApprovers = approvers
            .OrderBy(x => x.SequenceOrder)
            .ToList();

        if (assignedApprovers.Count == 0)
            throw new ArgumentException(
                "At least one approver is required.",
                nameof(approvers));

        if (assignedApprovers.Any(x =>
                x.ApproverEmployeeId == Guid.Empty))
            throw new ArgumentException(
                "Approver employee IDs are required.",
                nameof(approvers));

        if (assignedApprovers.Select(x => x.ApproverEmployeeId)
            .Distinct().Count() != assignedApprovers.Count)
            throw new ArgumentException(
                "An employee cannot appear more than once in an approval sequence.",
                nameof(approvers));

        for (var i = 0; i < assignedApprovers.Count; i++)
        {
            if (assignedApprovers[i].SequenceOrder != i + 1)
                throw new ArgumentException(
                    "Approval steps must be consecutive, starting at 1.",
                    nameof(approvers));
        }

        Id = Guid.NewGuid();
        RequisitionId = requisitionId;
        DepartmentId = departmentId;
        StartedAtUtc = DateTime.UtcNow;

        foreach (var approver in assignedApprovers)
        {
            _steps.Add(new RequisitionApprovalStep(
                Id,
                approver.ApproverEmployeeId,
                approver.SequenceOrder));
        }
    }

    public RequisitionApprovalStep GetCurrentStep()
    {
        if (IsCompleted || IsRejected)
            throw new InvalidOperationException(
                "This approval process is no longer active.");

        return _steps
            .OrderBy(x => x.SequenceOrder)
            .First(x => x.Status == ApprovalStepStatus.Pending);
    }

    public void ApproveCurrentStep(
        Guid employeeId,
        string? comments = null)
    {
        var step = GetCurrentStep();

        if (step.ApproverEmployeeId != employeeId)
            throw new InvalidOperationException(
                "Only the currently assigned approver can approve this step.");

        step.Approve(comments);

        if (_steps.All(x => x.Status == ApprovalStepStatus.Approved))
        {
            IsCompleted = true;
            CompletedAtUtc = DateTime.UtcNow;
        }
    }

    public void RejectCurrentStep(
        Guid employeeId,
        string? comments = null)
    {
        var step = GetCurrentStep();

        if (step.ApproverEmployeeId != employeeId)
            throw new InvalidOperationException(
                "Only the currently assigned approver can reject this step.");

        step.Reject(comments);

        IsRejected = true;
        CompletedAtUtc = DateTime.UtcNow;
    }
}