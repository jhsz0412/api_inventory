
using Approvals.Domain.Entities;
using Approvals.Domain.Enums;

namespace InventoryManagement.UnitTests;

public class RequisitionApprovalTests
{
    [Fact]
    public void ApproveCurrentStep_TwoApprovers_CompletesOnlyAfterBothApprove()
    {
        // Arrange
        var firstApproverId = Guid.NewGuid();
        var secondApproverId = Guid.NewGuid();

        var approval = new RequisitionApproval(
            Guid.NewGuid(),
            Guid.NewGuid(),
            [
                (firstApproverId, 1),
                (secondApproverId, 2)
            ]);

        // Act: the first approver approves
        approval.ApproveCurrentStep(firstApproverId, "First approval");

        // Assert: the second step is still pending
        Assert.False(approval.IsCompleted);
        Assert.Equal(
            ApprovalStepStatus.Approved,
            approval.Steps.Single(x => x.SequenceOrder == 1).Status);
        Assert.Equal(
            ApprovalStepStatus.Pending,
            approval.Steps.Single(x => x.SequenceOrder == 2).Status);

        // Act: the second approver approves
        approval.ApproveCurrentStep(secondApproverId, "Final approval");

        // Assert: the entire process is complete
        Assert.True(approval.IsCompleted);
        Assert.False(approval.IsRejected);
        Assert.NotNull(approval.CompletedAtUtc);
        Assert.All(
            approval.Steps,
            step => Assert.Equal(
                ApprovalStepStatus.Approved,
                step.Status));
    }

    [Fact]
    public void ApproveCurrentStep_WrongApprover_ThrowsInvalidOperationException()
    {
        // Arrange
        var assignedApproverId = Guid.NewGuid();
        var otherEmployeeId = Guid.NewGuid();

        var approval = new RequisitionApproval(
            Guid.NewGuid(),
            Guid.NewGuid(),
            [(assignedApproverId, 1)]);

        // Act and assert
        Assert.Throws<InvalidOperationException>(
            () => approval.ApproveCurrentStep(otherEmployeeId));
    }

    [Fact]
    public void RejectCurrentStep_AssignedApprover_RejectsApprovalProcess()
    {
        // Arrange
        var approverId = Guid.NewGuid();

        var approval = new RequisitionApproval(
            Guid.NewGuid(),
            Guid.NewGuid(),
            [(approverId, 1)]);

        // Act
        approval.RejectCurrentStep(approverId, "Please revise the request.");

        // Assert
        Assert.True(approval.IsRejected);
        Assert.False(approval.IsCompleted);
        Assert.NotNull(approval.CompletedAtUtc);
        Assert.Equal(
            ApprovalStepStatus.Rejected,
            approval.Steps.Single().Status);
        Assert.Equal(
            "Please revise the request.",
            approval.Steps.Single().Comments);
    }
}
