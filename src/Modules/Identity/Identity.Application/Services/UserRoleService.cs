using Identity.Application.DTOs.Roles;
using Identity.Application.Interfaces;
using Identity.Domain.Entities;

namespace Identity.Application.Services.UserRoles;

public sealed class UserRoleService(
    IUserRoleRepository repository) : IUserRoleService
{
    public async Task<IReadOnlyCollection<RoleResponse>> AssignRoleAsync(
        Guid userId,
        Guid roleId,
        CancellationToken cancellationToken = default)
    {
        if (userId == Guid.Empty)
            throw new ArgumentException("User ID is required.", nameof(userId));

        if (roleId == Guid.Empty)
            throw new ArgumentException("Role ID is required.", nameof(roleId));

        if (!await repository.UserExistsAsync(userId, cancellationToken))
            throw new KeyNotFoundException("User was not found.");

        if (!await repository.RoleExistsAsync(roleId, cancellationToken))
            throw new KeyNotFoundException("Role was not found.");

        if (await repository.AssignmentExistsAsync(
                userId, roleId, cancellationToken))
        {
            throw new InvalidOperationException(
                "This role is already assigned to the user.");
        }

        await repository.AddAsync(
            new UserRole
            {
                UserId = userId,
                RoleId = roleId,
                AssignedUtc = DateTimeOffset.UtcNow
            },
            cancellationToken);

        await repository.SaveChangesAsync(cancellationToken);

        return await GetRolesForUserAsync(userId, cancellationToken);
    }

    public async Task<IReadOnlyCollection<RoleResponse>> GetRolesForUserAsync(
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        if (userId == Guid.Empty)
            throw new ArgumentException("User ID is required.", nameof(userId));

        if (!await repository.UserExistsAsync(userId, cancellationToken))
            throw new KeyNotFoundException("User was not found.");

        var roles = await repository.GetRolesForUserAsync(
            userId, cancellationToken);

        return roles.Select(role => new RoleResponse
        {
            Id = role.Id,
            Name = role.Name,
            Description = role.Description,
            CreatedUtc = role.CreatedUtc
        }).ToArray();
    }
}
