
using Identity.Domain.Entities;

namespace Identity.Application.Interfaces;

public interface IPermissionRepository
{
    Task<bool> ExistsByCodeAsync(
        string code,
        CancellationToken cancellationToken = default);

    Task AddAsync(
        Permission permission,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyCollection<Permission>> GetAllAsync(
        CancellationToken cancellationToken = default);

    Task<Permission?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default);

    Task SaveChangesAsync(
        CancellationToken cancellationToken = default);
}
