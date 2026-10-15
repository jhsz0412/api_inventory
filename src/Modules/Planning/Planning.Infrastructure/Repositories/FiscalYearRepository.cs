
using Microsoft.EntityFrameworkCore;
using Planning.Application.Contracts;
using Planning.Domain.Entities;
using Planning.Infrastructure.Persistence;

namespace Planning.Infrastructure.Repositories;

public sealed class FiscalYearRepository(
    PlanningDbContext dbContext) : IFiscalYearRepository
{
    public async Task<IReadOnlyList<FiscalYear>> GetAllAsync(
        CancellationToken cancellationToken = default)
    {
        return await dbContext.FiscalYears
            .AsNoTracking()
            .OrderBy(fiscalYear => fiscalYear.StartDate)
            .ToListAsync(cancellationToken);
    }

    public async Task<FiscalYear?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        return await dbContext.FiscalYears
            .FirstOrDefaultAsync(
                fiscalYear => fiscalYear.Id == id,
                cancellationToken);
    }

    public async Task<bool> ExistsByNameAsync(
        string name,
        Guid? excludeId = null,
        CancellationToken cancellationToken = default)
    {
        var normalizedName = name.Trim();

        return await dbContext.FiscalYears.AnyAsync(
            fiscalYear =>
                fiscalYear.Name == normalizedName &&
                (!excludeId.HasValue ||
                 fiscalYear.Id != excludeId.Value),
            cancellationToken);
    }

    public async Task<bool> HasOverlapAsync(
        DateOnly startDate,
        DateOnly endDate,
        Guid? excludeId = null,
        CancellationToken cancellationToken = default)
    {
        return await dbContext.FiscalYears.AnyAsync(
            fiscalYear =>
                fiscalYear.StartDate <= endDate &&
                fiscalYear.EndDate >= startDate &&
                (!excludeId.HasValue ||
                 fiscalYear.Id != excludeId.Value),
            cancellationToken);
    }

    public async Task AddAsync(
        FiscalYear fiscalYear,
        CancellationToken cancellationToken = default)
    {
        await dbContext.FiscalYears.AddAsync(
            fiscalYear, cancellationToken);
    }

    public async Task SaveChangesAsync(
        CancellationToken cancellationToken = default)
    {
        await dbContext.SaveChangesAsync(cancellationToken);
    }
}
