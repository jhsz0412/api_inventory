
using Identity.Application.DTOs.Permissions;
using Identity.Application.Interfaces;
using Identity.Domain.Entities;

namespace Identity.Application.Services;

public sealed class RolePermissionService(
    IRolePermissionRepository rolePermissionRepository)
    : IRolePermissionService
{
    public async Task<IReadOnlyCollection<PermissionResponse>>
        AssignPermissionAsync(
            Guid roleId,
            Guid permissionId,
            CancellationToken cancellationToken = default)
    {
        if (roleId == Guid.Empty)
        {
            throw new ArgumentException(
                "Role ID cannot be empty.", nameof(roleId));
        }

        if (permissionId == Guid.Empty)
        {
            throw new ArgumentException(
                "Permission ID cannot be empty.", nameof(permissionId));
        }

        if (!await rolePermissionRepository.RoleExistsAsync(
                roleId, cancellationToken))
        {
            throw new KeyNotFoundException(
                $"Role '{roleId}' was not found.");
        }

        if (!await rolePermissionRepository.PermissionExistsAsync(
                permissionId, cancellationToken))
        {
            throw new KeyNotFoundException(
                $"Permission '{permissionId}' was not found.");
        }

        if (await rolePermissionRepository.AssignmentExistsAsync(
                roleId, permissionId, cancellationToken))
        {
            throw new InvalidOperationException(
                "This permission is already assigned to the role.");
        }

        var assignment = new RolePermission
        {
            RoleId = roleId,
            PermissionId = permissionId,
            AssignedUtc = DateTimeOffset.UtcNow
        };

        await rolePermissionRepository.AddAsync(
            assignment, cancellationToken);

        await rolePermissionRepository.SaveChangesAsync(
            cancellationToken);

        return await GetPermissionsAsync(roleId, cancellationToken);
    }

    public async Task<IReadOnlyCollection<PermissionResponse>>
        GetPermissionsAsync(
            Guid roleId,
            CancellationToken cancellationToken = default)
    {
        if (roleId == Guid.Empty)
        {
            throw new ArgumentException(
                "Role ID cannot be empty.", nameof(roleId));
        }

        if (!await rolePermissionRepository.RoleExistsAsync(
                roleId, cancellationToken))
        {
            throw new KeyNotFoundException(
                $"Role '{roleId}' was not found.");
        }

        var permissions =
            await rolePermissionRepository.GetPermissionsForRoleAsync(
                roleId, cancellationToken);

        return permissions.Select(permission => new PermissionResponse
        {
            Id = permission.Id,
            Code = permission.Code,
            Name = permission.Name,
            Description = permission.Description,
            CreatedUtc = permission.CreatedUtc
        }).ToArray();
    }
}

