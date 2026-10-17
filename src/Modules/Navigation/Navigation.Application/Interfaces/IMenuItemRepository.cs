
using Navigation.Domain.Entities;

namespace Navigation.Application.Interfaces;

public interface IMenuItemRepository
{
    Task<MenuItem?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<MenuItem>> GetAllAsync(
        CancellationToken cancellationToken = default);

    Task<bool> ExistsByNameAsync(
        string name,
        Guid? excludingId = null,
        CancellationToken cancellationToken = default);

    Task<bool> ParentExistsAsync(
        Guid parentId,
        CancellationToken cancellationToken = default);

    Task AddAsync(
        MenuItem menuItem,
        CancellationToken cancellationToken = default);

    void Update(MenuItem menuItem);

    Task SaveChangesAsync(
        CancellationToken cancellationToken = default);
}
