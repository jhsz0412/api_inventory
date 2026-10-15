
using Identity.Domain.Entities;

namespace Identity.Application.Interfaces;

public interface IRoleRepository
{
    Task<bool> ExistsByNameAsync(
        string name,
        CancellationToken cancellationToken = default);

    Task AddAsync(
        Role role,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyCollection<Role>> GetAllAsync(
        CancellationToken cancellationToken = default);

    Task<Role?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default);

    Task SaveChangesAsync(
        CancellationToken cancellationToken = default);
}
