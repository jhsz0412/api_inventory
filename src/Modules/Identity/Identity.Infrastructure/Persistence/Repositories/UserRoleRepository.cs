using Identity.Application.Interfaces;
using Identity.Domain.Entities;
using Identity.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Identity.Infrastructure.Persistence.Repositories;

public sealed class UserRoleRepository : IUserRoleRepository
{
    private readonly IdentityDbContext _dbContext;

    public UserRoleRepository(IdentityDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public Task<bool> UserExistsAsync(
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        return _dbContext.Users.AnyAsync(
            user => user.Id == userId,
            cancellationToken);
    }

    public Task<bool> RoleExistsAsync(
        Guid roleId,
        CancellationToken cancellationToken = default)
    {
        return _dbContext.Roles.AnyAsync(
            role => role.Id == roleId,
            cancellationToken);
    }

    public Task<bool> AssignmentExistsAsync(
        Guid userId,
        Guid roleId,
        CancellationToken cancellationToken = default)
    {
        return _dbContext.UserRoles.AnyAsync(
            assignment =>
                assignment.UserId == userId &&
                assignment.RoleId == roleId,
            cancellationToken);
    }

    public async Task AddAsync(
        UserRole userRole,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(userRole);

        await _dbContext.UserRoles.AddAsync(
            userRole,
            cancellationToken);
    }

    public async Task<IReadOnlyCollection<Role>> GetRolesForUserAsync(
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        return await _dbContext.UserRoles
            .AsNoTracking()
            .Where(assignment => assignment.UserId == userId)
            .Select(assignment => assignment.Role)
            .OrderBy(role => role.Name)
            .ToListAsync(cancellationToken);
    }

    public async Task SaveChangesAsync(
        CancellationToken cancellationToken = default)
    {
        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}
