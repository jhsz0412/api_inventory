
using Identity.Application.DTOs.Permissions;

namespace Identity.Application.Interfaces;

public interface IRolePermissionService
{
    Task<IReadOnlyCollection<PermissionResponse>> AssignPermissionAsync(
        Guid roleId,
        Guid permissionId,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyCollection<PermissionResponse>> GetPermissionsAsync(
        Guid roleId,
        CancellationToken cancellationToken = default);
}

