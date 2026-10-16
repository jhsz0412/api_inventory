namespace Approvals.Domain.Entities;

public sealed class DepartmentApprovalWorkflow
{
    public Guid Id { get; private set; }
    public Guid DepartmentId { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public bool IsActive { get; private set; }
    public DateTime CreatedAtUtc { get; private set; }
    public DateTime UpdatedAtUtc { get; private set; }

    private readonly List<DepartmentApprovalWorkflowStep> _steps = [];

    public IReadOnlyCollection<DepartmentApprovalWorkflowStep> Steps =>
        _steps.AsReadOnly();

    private DepartmentApprovalWorkflow()
    {
    }

    public DepartmentApprovalWorkflow(
        Guid departmentId,
        string name)
    {
        if (departmentId == Guid.Empty)
            throw new ArgumentException(
                "Department ID is required.",
                nameof(departmentId));

        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException(
                "Workflow name is required.",
                nameof(name));

        Id = Guid.NewGuid();
        DepartmentId = departmentId;
        Name = name.Trim();
        IsActive = false;
        CreatedAtUtc = DateTime.UtcNow;
        UpdatedAtUtc = CreatedAtUtc;
    }

    public void AddStep(Guid approverEmployeeId, int sequenceOrder)
    {
        if (approverEmployeeId == Guid.Empty)
            throw new ArgumentException(
                "Approver employee ID is required.",
                nameof(approverEmployeeId));

        if (sequenceOrder < 1)
            throw new ArgumentOutOfRangeException(
                nameof(sequenceOrder));

        if (_steps.Any(x => x.SequenceOrder == sequenceOrder))
            throw new InvalidOperationException(
                "The sequence order is already assigned.");

        if (_steps.Any(x => x.ApproverEmployeeId == approverEmployeeId))
            throw new InvalidOperationException(
                "An employee cannot be assigned more than once in the same workflow.");

        _steps.Add(new DepartmentApprovalWorkflowStep(
            Id,
            approverEmployeeId,
            sequenceOrder));

        UpdatedAtUtc = DateTime.UtcNow;
    }

    public void Activate()
    {
        if (_steps.Count == 0)
            throw new InvalidOperationException(
                "A workflow must contain at least one approver.");

        var ordered = _steps
            .OrderBy(x => x.SequenceOrder)
            .ToList();

        for (var i = 0; i < ordered.Count; i++)
        {
            if (ordered[i].SequenceOrder != i + 1)
                throw new InvalidOperationException(
                    "Approval steps must use consecutive sequence numbers starting at 1.");
        }

        IsActive = true;
        UpdatedAtUtc = DateTime.UtcNow;
    }

    public void Deactivate()
    {
        IsActive = false;
        UpdatedAtUtc = DateTime.UtcNow;
    }

    public void ReplaceConfiguration(
    string name,
    IEnumerable<DepartmentApprovalWorkflowStep> steps)
    {
        if (IsActive)
            throw new InvalidOperationException(
                "Deactivate the workflow before changing its configuration.");

        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Workflow name is required.", nameof(name));

        ArgumentNullException.ThrowIfNull(steps);

        var replacement = steps.OrderBy(x => x.SequenceOrder).ToList();

        if (replacement.Count == 0)
            throw new InvalidOperationException(
                "A workflow must contain at least one approver.");

        for (var i = 0; i < replacement.Count; i++)
            if (replacement[i].SequenceOrder != i + 1)
                throw new InvalidOperationException(
                    "Approval steps must be consecutive, starting at 1.");

        if (replacement.Select(x => x.ApproverEmployeeId).Distinct().Count()
            != replacement.Count)
            throw new InvalidOperationException(
                "An employee cannot appear more than once in a workflow.");

        Name = name.Trim();
        _steps.Clear();

        foreach (var step in replacement)
            _steps.Add(new DepartmentApprovalWorkflowStep(
                Id, step.ApproverEmployeeId, step.SequenceOrder));

        UpdatedAtUtc = DateTime.UtcNow;
    }
}