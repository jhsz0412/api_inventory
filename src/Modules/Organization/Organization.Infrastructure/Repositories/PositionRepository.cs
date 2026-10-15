
using Microsoft.EntityFrameworkCore;
using Organization.Application.Contracts;
using Organization.Domain.Entities;
using Organization.Infrastructure.Persistence;

namespace Organization.Infrastructure.Repositories;

public sealed class PositionRepository(
    OrganizationDbContext dbContext) : IPositionRepository
{
    public async Task<IReadOnlyList<Position>> GetAllAsync(
        CancellationToken cancellationToken = default)
    {
        return await dbContext.Positions
            .AsNoTracking()
            .OrderBy(position => position.Name)
            .ToListAsync(cancellationToken);
    }

    public async Task<Position?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        return await dbContext.Positions
            .FirstOrDefaultAsync(
                position => position.Id == id,
                cancellationToken);
    }

    public async Task<bool> ExistsByCodeAsync(
        string code,
        Guid? excludeId = null,
        CancellationToken cancellationToken = default)
    {
        return await dbContext.Positions.AnyAsync(
            position =>
                position.Code == code &&
                (!excludeId.HasValue || position.Id != excludeId.Value),
            cancellationToken);
    }

    public async Task AddAsync(
        Position position,
        CancellationToken cancellationToken = default)
    {
        await dbContext.Positions.AddAsync(position, cancellationToken);
    }

    public async Task SaveChangesAsync(
        CancellationToken cancellationToken = default)
    {
        await dbContext.SaveChangesAsync(cancellationToken);
    }
}
