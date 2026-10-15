
using Identity.Domain.Entities;

namespace Identity.Application.Interfaces;

public interface IUserRoleRepository
{
    Task<bool> UserExistsAsync(
        Guid userId,
        CancellationToken cancellationToken = default);

    Task<bool> RoleExistsAsync(
        Guid roleId,
        CancellationToken cancellationToken = default);

    Task<bool> AssignmentExistsAsync(
        Guid userId,
        Guid roleId,
        CancellationToken cancellationToken = default);

    Task AddAsync(
        UserRole userRole,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyCollection<Role>> GetRolesForUserAsync(
        Guid userId,
        CancellationToken cancellationToken = default);

    Task SaveChangesAsync(
        CancellationToken cancellationToken = default);
}
