
using Identity.Domain.Entities;
using Identity.Infrastructure.Persistence;
using Identity.Infrastructure.Security;
using Microsoft.EntityFrameworkCore;
using Organization.Infrastructure.Persistence;
using System.Globalization;

namespace InventoryManagement.DatabaseSetup.Seeding;

public static class IdentitySeeder
{
    private const string AdminUserName = "Administrator";
    private const string AdminEmail = "administrator@inventory.local";
    private const string AdminRoleName = "System Administrator";

    private static readonly string[] PermissionCodes =
    [
        "identity.users.manage",
        "identity.roles.manage",
        "identity.users.roles.manage",
        "identity.permissions.manage",

        "departments.read",
        "departments.create",
        "departments.update",

        "planning.fiscal-years.read",
        "planning.fiscal-years.create",
        "planning.fiscal-years.update",

        "requisitions.create",
        "requisitions.read",
        "requisitions.update",
        "requisitions.submit",
        "requisitions.approve",
        "requisitions.reject",
        "requisitions.cancel",
        "requisitions.fulfill",

        "approvals.workflows.read",
        "approvals.workflows.manage",

        "inventory.categories.read",
        "inventory.categories.create",
        "inventory.items.read",
        "inventory.items.create",
        "inventory.stock.read",
        "inventory.stock.receive",
        "inventory.stock.issue",
        "inventory.stock.adjust",
        "inventory.storage-locations.create",
        "inventory.storage-locations.read",
        "inventory.storage-locations.update",
        "inventory.storage-locations.manage",
        "inventory.units.read",
        "inventory.units.create",
        "inventory.warehouses.create",
        "inventory.warehouses.read",
        "inventory.warehouses.update",

        "deliveries.read",
        "deliveries.create",
        "deliveries.update",
        "deliveries.dispatch",
        "deliveries.recover",
        "deliver.cancel"
    ];

    public static async Task SeedAsync(
        IdentityDbContext identityDb,
        OrganizationDbContext organizationDb,
        CancellationToken cancellationToken = default)
    {
        // Find the organization employee created by OrganizationSeeder.
        var employee = await organizationDb.Employees
            .SingleOrDefaultAsync(
                x => x.EmployeeNumber == "ADMIN-001",
                cancellationToken);

        if (employee is null)
        {
            throw new InvalidOperationException(
                "Employee ADMIN-001 was not found. " +
                "Run OrganizationSeeder before IdentitySeeder.");
        }

        // Create or reuse permissions.
        var existingPermissions = await identityDb.Permissions
            .ToDictionaryAsync(
                x => x.Code,
                cancellationToken);

        foreach (var code in PermissionCodes)
        {
            if (existingPermissions.ContainsKey(code))
            {
                continue;
            }

            var name = CultureInfo.InvariantCulture.TextInfo
                .ToTitleCase(code.Replace('.', ' ').Replace('-', ' '));

            var permission = new Permission
            {
                Code = code,
                Name = name,
                Description = $"Allows the user to perform: {name}."
            };

            identityDb.Permissions.Add(permission);
            existingPermissions.Add(code, permission);
        }

        await identityDb.SaveChangesAsync(cancellationToken);

        // Create or reuse the System Administrator role.
        var role = await identityDb.Roles
            .SingleOrDefaultAsync(
                x => x.Name == AdminRoleName,
                cancellationToken);

        if (role is null)
        {
            role = new Role
            {
                Name = AdminRoleName,
                Description = "Full access to Inventory Management."
            };

            identityDb.Roles.Add(role);
            await identityDb.SaveChangesAsync(cancellationToken);
        }

        // Assign every seeded permission to the Administrator role.
        var assignedPermissionIds = await identityDb.RolePermissions
            .Where(x => x.RoleId == role.Id)
            .Select(x => x.PermissionId)
            .ToListAsync(cancellationToken);

        var assignedSet = assignedPermissionIds.ToHashSet();

        foreach (var permission in existingPermissions.Values)
        {
            if (assignedSet.Add(permission.Id))
            {
                identityDb.RolePermissions.Add(new RolePermission
                {
                    RoleId = role.Id,
                    PermissionId = permission.Id
                });
            }
        }

        await identityDb.SaveChangesAsync(cancellationToken);

        // Create or reuse the Administrator account.
        var user = await identityDb.Users
            .SingleOrDefaultAsync(
                x => x.UserName == AdminUserName,
                cancellationToken);

        if (user is null)
        {
            var emailInUse = await identityDb.Users
                .AnyAsync(
                    x => x.Email == AdminEmail,
                    cancellationToken);

            if (emailInUse)
            {
                throw new InvalidOperationException(
                    $"The email {AdminEmail} is already used by another account.");
            }

            var password = Environment.GetEnvironmentVariable(
                "INVENTORY_ADMIN_PASSWORD");

            if (string.IsNullOrWhiteSpace(password))
            {
                throw new InvalidOperationException(
                    "Set the INVENTORY_ADMIN_PASSWORD environment variable " +
                    "before creating the Administrator account.");
            }

            if (password.Length < 12)
            {
                throw new InvalidOperationException(
                    "The Administrator password must be at least 12 characters.");
            }

            user = new User
            {
                EmployeeId = employee.Id,
                UserName = AdminUserName,
                Email = AdminEmail,
                PasswordHash = new PasswordHashingService()
                    .HashPassword(password),
                IsActive = true
            };

            identityDb.Users.Add(user);
            await identityDb.SaveChangesAsync(cancellationToken);
        }
        else if (user.EmployeeId != employee.Id)
        {
            throw new InvalidOperationException(
                "The existing Administrator account is linked to a different " +
                "employee. No account changes were made.");
        }

        // Assign the role to the Administrator user.
        var alreadyAssigned = await identityDb.UserRoles
            .AnyAsync(
                x => x.UserId == user.Id && x.RoleId == role.Id,
                cancellationToken);

        if (!alreadyAssigned)
        {
            identityDb.UserRoles.Add(new UserRole
            {
                UserId = user.Id,
                RoleId = role.Id
            });

            await identityDb.SaveChangesAsync(cancellationToken);
        }

        Console.WriteLine(
            $"Identity defaults ready. User: {user.UserName}; Role: {role.Name}");


        // Create or reuse the separate Requisition Approver account.
        var approverEmployee = await organizationDb.Employees
            .SingleOrDefaultAsync(
                x => x.EmployeeNumber == "APPROVER-001",
                cancellationToken);

        if (approverEmployee is null)
        {
            throw new InvalidOperationException(
                "Employee APPROVER-001 was not found. " +
                "Run OrganizationSeeder first.");
        }

        const string approverUserName = "RequisitionApprover";
        const string approverEmail =
            "requisition.approver@inventory.local";
        const string approverRoleName = "Requisition Approver";

        var approverRole = await identityDb.Roles
            .SingleOrDefaultAsync(
                x => x.Name == approverRoleName,
                cancellationToken);

        if (approverRole is null)
        {
            approverRole = new Role
            {
                Name = approverRoleName,
                Description =
                    "Reviews and approves or rejects requisitions."
            };

            identityDb.Roles.Add(approverRole);
            await identityDb.SaveChangesAsync(cancellationToken);
        }

        // Ensure the required permissions are assigned to this role.
        var approverPermissionCodes = new[]
        {
            "requisitions.read",
            "requisitions.approve",
            "requisitions.reject"
        };

        foreach (var code in approverPermissionCodes)
        {
            if (!existingPermissions.TryGetValue(
                    code, out var permission))
            {
                throw new InvalidOperationException(
                    $"Required permission '{code}' was not seeded.");
            }

            var assigned = await identityDb.RolePermissions
                .AnyAsync(
                    x => x.RoleId == approverRole.Id &&
                         x.PermissionId == permission.Id,
                    cancellationToken);

            if (!assigned)
            {
                identityDb.RolePermissions.Add(new RolePermission
                {
                    RoleId = approverRole.Id,
                    PermissionId = permission.Id
                });
            }
        }

        await identityDb.SaveChangesAsync(cancellationToken);

        var approverUser = await identityDb.Users
            .SingleOrDefaultAsync(
                x => x.UserName == approverUserName,
                cancellationToken);

        if (approverUser is null)
        {
            var emailInUse = await identityDb.Users
                .AnyAsync(
                    x => x.Email == approverEmail,
                    cancellationToken);

            if (emailInUse)
            {
                throw new InvalidOperationException(
                    "The approver email is already used by another account.");
            }

            var approverPassword = Environment.GetEnvironmentVariable(
                "INVENTORY_APPROVER_PASSWORD");

            if (string.IsNullOrWhiteSpace(approverPassword))
            {
                throw new InvalidOperationException(
                    "Set INVENTORY_APPROVER_PASSWORD in the environment " +
                    "before creating the approver account.");
            }

            if (approverPassword.Length < 12)
            {
                throw new InvalidOperationException(
                    "The approver password must be at least 12 characters.");
            }

            approverUser = new User
            {
                EmployeeId = approverEmployee.Id,
                UserName = approverUserName,
                Email = approverEmail,
                PasswordHash = new PasswordHashingService()
                    .HashPassword(approverPassword),
                IsActive = true
            };

            identityDb.Users.Add(approverUser);
            await identityDb.SaveChangesAsync(cancellationToken);
        }
        else if (approverUser.EmployeeId != approverEmployee.Id)
        {
            throw new InvalidOperationException(
                "The existing approver account is linked to a different " +
                "employee. No account relinking was performed.");
        }

        var approverRoleAssigned = await identityDb.UserRoles
            .AnyAsync(
                x => x.UserId == approverUser.Id &&
                     x.RoleId == approverRole.Id,
                cancellationToken);

        if (!approverRoleAssigned)
        {
            identityDb.UserRoles.Add(new UserRole
            {
                UserId = approverUser.Id,
                RoleId = approverRole.Id
            });

            await identityDb.SaveChangesAsync(cancellationToken);
        }

        Console.WriteLine(
            $"Approver account ready: {approverUser.UserName}; " +
            $"Role: {approverRole.Name}");

    }
}

