using Organization.Domain.Entities;

namespace Organization.Application.Contracts;

public interface IGroupRepository
{
    Task<IReadOnlyList<Group>> GetAllAsync(
        CancellationToken cancellationToken = default);

    Task<Group?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default);

    Task<bool> ExistsByCodeAsync(
        string code,
        Guid departmentId,
        Guid? excludeId = null,
        CancellationToken cancellationToken = default);

    Task<bool> DepartmentExistsAsync(
        Guid departmentId,
        CancellationToken cancellationToken = default);

    Task AddAsync(
        Group group,
        CancellationToken cancellationToken = default);

    Task SaveChangesAsync(
        CancellationToken cancellationToken = default);
}