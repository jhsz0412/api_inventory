
using Organization.Domain.Entities;

namespace Organization.Application.Contracts;

public interface IDepartmentRepository
{
    Task<IReadOnlyList<Department>> GetAllAsync(
        CancellationToken cancellationToken = default);

    Task<Department?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default);

    Task<bool> ExistsByCodeAsync(
        string code,
        Guid? excludeId = null,
        CancellationToken cancellationToken = default);

    Task AddAsync(
        Department department,
        CancellationToken cancellationToken = default);

    Task SaveChangesAsync(
        CancellationToken cancellationToken = default);
}
