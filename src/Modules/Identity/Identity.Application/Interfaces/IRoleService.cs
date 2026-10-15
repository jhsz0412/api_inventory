
using Identity.Application.DTOs.Roles;

namespace Identity.Application.Interfaces;

public interface IRoleService
{
    Task<RoleResponse> CreateAsync(
        CreateRoleRequest request,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyCollection<RoleResponse>> GetAllAsync(
        CancellationToken cancellationToken = default);

    Task<RoleResponse?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default);
}
