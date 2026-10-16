using Approvals.Domain.Entities;
using Approvals.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Organization.Infrastructure.Persistence;

namespace InventoryManagement.DatabaseSetup.Seeding;

public static class ApprovalWorkflowSeeder
{
    private const string WorkflowName =
        "Administration Requisition Approval";

    public static async Task SeedAsync(
        ApprovalsDbContext approvalsDb,
        OrganizationDbContext organizationDb,
        CancellationToken cancellationToken = default)
    {
        var department = await organizationDb.Departments
            .SingleOrDefaultAsync(
                x => x.Code == "ADMIN",
                cancellationToken);

        if (department is null)
        {
            throw new InvalidOperationException(
                "Department ADMIN was not found. " +
                "Run OrganizationSeeder first.");
        }

        var approver = await organizationDb.Employees
            .SingleOrDefaultAsync(
                x => x.EmployeeNumber == "APPROVER-001",
                cancellationToken);

        if (approver is null)
        {
            throw new InvalidOperationException(
                "Employee APPROVER-001 was not found. " +
                "Run OrganizationSeeder first.");
        }

        var existingWorkflow = await approvalsDb
            .DepartmentApprovalWorkflows
            .Include(x => x.Steps)
            .SingleOrDefaultAsync(
                x => x.DepartmentId == department.Id &&
                     x.Name == WorkflowName,
                cancellationToken);

        if (existingWorkflow is not null)
        {
            Console.WriteLine(
                $"Approval workflow already exists: {WorkflowName}. " +
                "Existing workflow was left unchanged.");

            return;
        }

        var workflow = new DepartmentApprovalWorkflow(
            department.Id,
            WorkflowName);

        workflow.AddStep(approver.Id, 1);
        workflow.Activate();

        approvalsDb.DepartmentApprovalWorkflows.Add(workflow);

        await approvalsDb.SaveChangesAsync(cancellationToken);

        Console.WriteLine(
            $"Approval workflow created and activated: {WorkflowName}");

        Console.WriteLine(
            $"Step 1 approver: {approver.EmployeeNumber}");
    }
}