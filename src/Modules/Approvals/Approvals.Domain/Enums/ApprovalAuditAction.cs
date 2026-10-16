namespace Approvals.Domain.Enums;

public enum ApprovalAuditAction
{
    WorkflowCreated = 1,
    WorkflowUpdated = 2,
    ApprovalStarted = 3,
    StepApproved = 4,
    StepRejected = 5,
    ApprovalCompleted = 6,
    ApprovalCancelled = 7
}