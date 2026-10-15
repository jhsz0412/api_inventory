
using Identity.Application.DTOs.Permissions;

namespace Identity.Application.Interfaces;

public interface IPermissionService
{
    Task<PermissionResponse> CreateAsync(
        CreatePermissionRequest request,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyCollection<PermissionResponse>> GetAllAsync(
        CancellationToken cancellationToken = default);

    Task<PermissionResponse?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default);
}
