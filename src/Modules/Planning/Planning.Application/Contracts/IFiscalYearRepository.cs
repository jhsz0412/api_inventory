
using Planning.Domain.Entities;

namespace Planning.Application.Contracts;

public interface IFiscalYearRepository
{
    Task<IReadOnlyList<FiscalYear>> GetAllAsync(
        CancellationToken cancellationToken = default);

    Task<FiscalYear?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default);

    Task<bool> ExistsByNameAsync(
        string name,
        Guid? excludeId = null,
        CancellationToken cancellationToken = default);

    Task<bool> HasOverlapAsync(
        DateOnly startDate,
        DateOnly endDate,
        Guid? excludeId = null,
        CancellationToken cancellationToken = default);

    Task AddAsync(
        FiscalYear fiscalYear,
        CancellationToken cancellationToken = default);

    Task SaveChangesAsync(
        CancellationToken cancellationToken = default);
}
