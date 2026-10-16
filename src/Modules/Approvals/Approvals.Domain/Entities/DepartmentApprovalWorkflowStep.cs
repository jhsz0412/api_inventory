namespace Approvals.Domain.Entities;

public sealed class DepartmentApprovalWorkflowStep
{
    public Guid Id { get; private set; }
    public Guid DepartmentApprovalWorkflowId { get; private set; }
    public Guid ApproverEmployeeId { get; private set; }
    public int SequenceOrder { get; private set; }

    private DepartmentApprovalWorkflowStep()
    {
    }

    internal DepartmentApprovalWorkflowStep(
        Guid workflowId,
        Guid approverEmployeeId,
        int sequenceOrder)
    {
        Id = Guid.NewGuid();
        DepartmentApprovalWorkflowId = workflowId;
        ApproverEmployeeId = approverEmployeeId;
        SequenceOrder = sequenceOrder;
    }
}
