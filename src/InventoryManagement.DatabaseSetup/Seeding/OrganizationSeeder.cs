
using Microsoft.EntityFrameworkCore;
using Organization.Domain.Entities;
using Organization.Infrastructure.Persistence;

namespace InventoryManagement.DatabaseSetup.Seeding;

public static class OrganizationSeeder
{
    public static async Task SeedAsync(
        OrganizationDbContext db,
        CancellationToken cancellationToken = default)
    {
        // 1. Create or reuse the Administration department.
        var department = await db.Departments
            .SingleOrDefaultAsync(
                x => x.Code == "ADMIN",
                cancellationToken);

        if (department is null)
        {
            department = new Department(
                "ADMIN",
                "Administration");

            db.Departments.Add(department);
            await db.SaveChangesAsync(cancellationToken);
        }

        // 2. Create or reuse the Administrator position.
        var position = await db.Positions
            .SingleOrDefaultAsync(
                x => x.Code == "SYSADMIN",
                cancellationToken);

        if (position is null)
        {
            position = new Position(
                "SYSADMIN",
                "System Administrator");

            db.Positions.Add(position);
            await db.SaveChangesAsync(cancellationToken);
        }

        // 3. Create or reuse the Administrator employee.
        var employee = await db.Employees
            .SingleOrDefaultAsync(
                x => x.EmployeeNumber == "ADMIN-001",
                cancellationToken);

        if (employee is null)
        {
            employee = new Employee(
                employeeNumber: "ADMIN-001",
                firstName: "System",
                lastName: "Administrator",
                departmentId: department.Id,
                positionId: position.Id,
                email: "administrator@inventory.local");

            db.Employees.Add(employee);
            await db.SaveChangesAsync(cancellationToken);
        }

        Console.WriteLine(
            $"Organization defaults ready. Employee: {employee.EmployeeNumber}");


        // 4. Create or reuse the Requisition Approver position.
        var approverPosition = await db.Positions
            .SingleOrDefaultAsync(
                x => x.Code == "REQ-APPROVER",
                cancellationToken);

        if (approverPosition is null)
        {
            approverPosition = new Position(
                "REQ-APPROVER",
                "Requisition Approver");

            db.Positions.Add(approverPosition);
            await db.SaveChangesAsync(cancellationToken);
        }

        // 5. Create or reuse the Requisition Approver employee.
        var approverEmployee = await db.Employees
            .SingleOrDefaultAsync(
                x => x.EmployeeNumber == "APPROVER-001",
                cancellationToken);

        if (approverEmployee is null)
        {
            approverEmployee = new Employee(
                employeeNumber: "APPROVER-001",
                firstName: "Requisition",
                lastName: "Approver",
                departmentId: department.Id,
                positionId: approverPosition.Id,
                email: "requisition.approver@inventory.local");

            db.Employees.Add(approverEmployee);
            await db.SaveChangesAsync(cancellationToken);
        }

        Console.WriteLine(
            $"Default approver employee ready: {approverEmployee.EmployeeNumber}");

    }
}
