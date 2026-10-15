
using Identity.Application.DTOs.Roles;
using Identity.Application.Interfaces;
using Identity.Domain.Entities;

namespace Identity.Application.Services;

public sealed class RoleService(
    IRoleRepository roleRepository) : IRoleService
{
    public async Task<RoleResponse> CreateAsync(
        CreateRoleRequest request,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        var name = request.Name.Trim();

        if (name.Length < 2 || name.Length > 100)
        {
            throw new ArgumentException(
                "Role name must contain between 2 and 100 characters.",
                nameof(request));
        }

        var description = string.IsNullOrWhiteSpace(request.Description)
            ? null
            : request.Description.Trim();

        if (description?.Length > 500)
        {
            throw new ArgumentException(
                "Role description cannot exceed 500 characters.",
                nameof(request));
        }

        if (await roleRepository.ExistsByNameAsync(
                name, cancellationToken))
        {
            throw new InvalidOperationException(
                $"A role named '{name}' already exists.");
        }

        var role = new Role
        {
            Name = name,
            Description = description,
            CreatedUtc = DateTimeOffset.UtcNow
        };

        await roleRepository.AddAsync(role, cancellationToken);
        await roleRepository.SaveChangesAsync(cancellationToken);

        return MapToResponse(role);
    }

    public async Task<IReadOnlyCollection<RoleResponse>> GetAllAsync(
        CancellationToken cancellationToken = default)
    {
        var roles = await roleRepository.GetAllAsync(cancellationToken);

        return roles.Select(MapToResponse).ToArray();
    }

    public async Task<RoleResponse?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        if (id == Guid.Empty)
        {
            throw new ArgumentException(
                "Role ID cannot be empty.", nameof(id));
        }

        var role = await roleRepository.GetByIdAsync(
            id, cancellationToken);

        return role is null ? null : MapToResponse(role);
    }

    private static RoleResponse MapToResponse(Role role)
    {
        return new RoleResponse
        {
            Id = role.Id,
            Name = role.Name,
            Description = role.Description,
            CreatedUtc = role.CreatedUtc
        };
    }
}
