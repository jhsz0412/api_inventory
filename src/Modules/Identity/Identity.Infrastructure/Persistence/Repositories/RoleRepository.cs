
using Identity.Application.Interfaces;
using Identity.Domain.Entities;
using Identity.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Identity.Infrastructure.Persistence.Repositories;

public sealed class RoleRepository(
    IdentityDbContext dbContext) : IRoleRepository
{
    public Task<bool> ExistsByNameAsync(
        string name,
        CancellationToken cancellationToken = default)
    {
        return dbContext.Roles.AnyAsync(
            role => role.Name == name,
            cancellationToken);
    }

    public async Task AddAsync(
        Role role,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(role);

        await dbContext.Roles.AddAsync(role, cancellationToken);
    }

    public async Task<IReadOnlyCollection<Role>> GetAllAsync(
        CancellationToken cancellationToken = default)
    {
        return await dbContext.Roles
            .AsNoTracking()
            .OrderBy(role => role.Name)
            .ToListAsync(cancellationToken);
    }

    public Task<Role?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        return dbContext.Roles
            .AsNoTracking()
            .FirstOrDefaultAsync(
                role => role.Id == id,
                cancellationToken);
    }

    public async Task SaveChangesAsync(
        CancellationToken cancellationToken = default)
    {
        await dbContext.SaveChangesAsync(cancellationToken);
    }
}
