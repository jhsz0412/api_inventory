
using Identity.Application.Interfaces;
using Identity.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Identity.Infrastructure.Persistence.Repositories;

public sealed class PermissionRepository(
    IdentityDbContext dbContext) : IPermissionRepository
{
    public Task<bool> ExistsByCodeAsync(
        string code,
        CancellationToken cancellationToken = default)
    {
        return dbContext.Permissions.AnyAsync(
            permission => permission.Code == code,
            cancellationToken);
    }

    public async Task AddAsync(
        Permission permission,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(permission);

        await dbContext.Permissions.AddAsync(
            permission, cancellationToken);
    }

    public async Task<IReadOnlyCollection<Permission>> GetAllAsync(
        CancellationToken cancellationToken = default)
    {
        return await dbContext.Permissions
            .AsNoTracking()
            .OrderBy(permission => permission.Code)
            .ToListAsync(cancellationToken);
    }

    public Task<Permission?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        return dbContext.Permissions
            .AsNoTracking()
            .FirstOrDefaultAsync(
                permission => permission.Id == id,
                cancellationToken);
    }

    public async Task SaveChangesAsync(
        CancellationToken cancellationToken = default)
    {
        await dbContext.SaveChangesAsync(cancellationToken);
    }
}
