
using Identity.Domain.Entities;

namespace Identity.Application.Interfaces;

public interface IRolePermissionRepository
{
    Task<bool> RoleExistsAsync(
        Guid roleId,
        CancellationToken cancellationToken = default);

    Task<bool> PermissionExistsAsync(
        Guid permissionId,
        CancellationToken cancellationToken = default);

    Task<bool> AssignmentExistsAsync(
        Guid roleId,
        Guid permissionId,
        CancellationToken cancellationToken = default);

    Task AddAsync(
        RolePermission rolePermission,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyCollection<Permission>> GetPermissionsForRoleAsync(
        Guid roleId,
        CancellationToken cancellationToken = default);

    Task SaveChangesAsync(
        CancellationToken cancellationToken = default);
}
