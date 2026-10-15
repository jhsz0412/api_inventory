
using Organization.Domain.Entities;

namespace Organization.Application.Contracts;

public interface IPositionRepository
{
    Task<IReadOnlyList<Position>> GetAllAsync(
        CancellationToken cancellationToken = default);

    Task<Position?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default);

    Task<bool> ExistsByCodeAsync(
        string code,
        Guid? excludeId = null,
        CancellationToken cancellationToken = default);

    Task AddAsync(
        Position position,
        CancellationToken cancellationToken = default);

    Task SaveChangesAsync(
        CancellationToken cancellationToken = default);
}
