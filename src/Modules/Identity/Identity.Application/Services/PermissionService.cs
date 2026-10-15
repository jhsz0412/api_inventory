
using Identity.Application.DTOs.Permissions;
using Identity.Application.Interfaces;
using Identity.Domain.Entities;

namespace Identity.Application.Services;

public sealed class PermissionService(
    IPermissionRepository permissionRepository) : IPermissionService
{
    public async Task<PermissionResponse> CreateAsync(
        CreatePermissionRequest request,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        var code = request.Code.Trim().ToLowerInvariant();
        var name = request.Name.Trim();

        if (code.Length < 3 || code.Length > 150)
        {
            throw new ArgumentException(
                "Permission code must contain between 3 and 150 characters.",
                nameof(request));
        }

        if (name.Length < 2 || name.Length > 150)
        {
            throw new ArgumentException(
                "Permission name must contain between 2 and 150 characters.",
                nameof(request));
        }

        if (code.Any(char.IsWhiteSpace))
        {
            throw new ArgumentException(
                "Permission code cannot contain whitespace.",
                nameof(request));
        }

        var description = string.IsNullOrWhiteSpace(request.Description)
            ? null
            : request.Description.Trim();

        if (description?.Length > 500)
        {
            throw new ArgumentException(
                "Permission description cannot exceed 500 characters.",
                nameof(request));
        }

        if (await permissionRepository.ExistsByCodeAsync(
                code, cancellationToken))
        {
            throw new InvalidOperationException(
                $"A permission with code '{code}' already exists.");
        }

        var permission = new Permission
        {
            Code = code,
            Name = name,
            Description = description,
            CreatedUtc = DateTimeOffset.UtcNow
        };

        await permissionRepository.AddAsync(
            permission, cancellationToken);

        await permissionRepository.SaveChangesAsync(cancellationToken);

        return MapToResponse(permission);
    }

    public async Task<IReadOnlyCollection<PermissionResponse>> GetAllAsync(
        CancellationToken cancellationToken = default)
    {
        var permissions = await permissionRepository.GetAllAsync(
            cancellationToken);

        return permissions.Select(MapToResponse).ToArray();
    }

    public async Task<PermissionResponse?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        if (id == Guid.Empty)
        {
            throw new ArgumentException(
                "Permission ID cannot be empty.", nameof(id));
        }

        var permission = await permissionRepository.GetByIdAsync(
            id, cancellationToken);

        return permission is null ? null : MapToResponse(permission);
    }

    private static PermissionResponse MapToResponse(Permission permission)
    {
        return new PermissionResponse
        {
            Id = permission.Id,
            Code = permission.Code,
            Name = permission.Name,
            Description = permission.Description,
            CreatedUtc = permission.CreatedUtc
        };
    }
}
