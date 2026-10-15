
using Identity.Application.Interfaces;
using Identity.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Identity.Infrastructure.Persistence.Repositories;

public sealed class RolePermissionRepository(
    IdentityDbContext dbContext) : IRolePermissionRepository
{
    public Task<bool> RoleExistsAsync(
        Guid roleId,
        CancellationToken cancellationToken = default)
    {
        return dbContext.Roles.AnyAsync(
            role => role.Id == roleId,
            cancellationToken);
    }

    public Task<bool> PermissionExistsAsync(
        Guid permissionId,
        CancellationToken cancellationToken = default)
    {
        return dbContext.Permissions.AnyAsync(
            permission => permission.Id == permissionId,
            cancellationToken);
    }

    public Task<bool> AssignmentExistsAsync(
        Guid roleId,
        Guid permissionId,
        CancellationToken cancellationToken = default)
    {
        return dbContext.RolePermissions.AnyAsync(
            assignment =>
                assignment.RoleId == roleId &&
                assignment.PermissionId == permissionId,
            cancellationToken);
    }

    public async Task AddAsync(
        RolePermission rolePermission,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(rolePermission);

        await dbContext.RolePermissions.AddAsync(
            rolePermission, cancellationToken);
    }

    public async Task<IReadOnlyCollection<Permission>>
        GetPermissionsForRoleAsync(
            Guid roleId,
            CancellationToken cancellationToken = default)
    {
        return await dbContext.RolePermissions
            .AsNoTracking()
            .Where(assignment => assignment.RoleId == roleId)
            .Select(assignment => assignment.Permission)
            .OrderBy(permission => permission.Code)
            .ToListAsync(cancellationToken);
    }

    public async Task SaveChangesAsync(
        CancellationToken cancellationToken = default)
    {
        await dbContext.SaveChangesAsync(cancellationToken);
    }
}

