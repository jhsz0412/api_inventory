using Identity.Application.DTOs.Roles;

namespace Identity.Application.Interfaces;

public interface IUserRoleService
{
    Task<IReadOnlyCollection<RoleResponse>> AssignRoleAsync(
        Guid userId,
        Guid roleId,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyCollection<RoleResponse>> GetRolesForUserAsync(
        Guid userId,
        CancellationToken cancellationToken = default);
}
