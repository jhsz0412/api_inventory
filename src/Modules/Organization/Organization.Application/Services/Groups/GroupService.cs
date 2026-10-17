
using Organization.Application.Contracts;
using Organization.Application.DTOs.Groups;
using Organization.Domain.Entities;

namespace Organization.Application.Services.Groups;

public sealed class GroupService(
    IGroupRepository repository) : IGroupService
{
    public async Task<IReadOnlyList<GroupResponse>> GetAllAsync(
        CancellationToken cancellationToken = default)
    {
        var groups = await repository.GetAllAsync(cancellationToken);

        return groups.Select(MapToResponse).ToList();
    }

    public async Task<GroupResponse?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        var group = await repository.GetByIdAsync(
            id, cancellationToken);

        return group is null ? null : MapToResponse(group);
    }

    public async Task<GroupResponse> CreateAsync(
        CreateGroupRequest request,
        CancellationToken cancellationToken = default)
    {
        var code = NormalizeCode(request.Code);
        var name = NormalizeName(request.Name);

        Validate(code, name);

        if (await repository.ExistsByCodeAsync(
                code,
                cancellationToken: cancellationToken))
        {
            throw new InvalidOperationException(
                $"Group code '{code}' already exists.");
        }

        var group = new Group(code, name);

        await repository.AddAsync(group, cancellationToken);
        await repository.SaveChangesAsync(cancellationToken);

        return MapToResponse(group);
    }

    public async Task<GroupResponse?> UpdateAsync(
        Guid id,
        UpdateGroupRequest request,
        CancellationToken cancellationToken = default)
    {
        var group = await repository.GetByIdAsync(
            id, cancellationToken);

        if (group is null)
            return null;

        var code = NormalizeCode(request.Code);
        var name = NormalizeName(request.Name);

        Validate(code, name);

        if (await repository.ExistsByCodeAsync(
                code,
                id,
                cancellationToken))
        {
            throw new InvalidOperationException(
                $"Group code '{code}' already exists.");
        }

        group.UpdateDetails(code, name);

        await repository.SaveChangesAsync(cancellationToken);

        return MapToResponse(group);
    }

    public async Task<bool> SetStatusAsync(
        Guid id,
        bool isActive,
        CancellationToken cancellationToken = default)
    {
        var group = await repository.GetByIdAsync(
            id, cancellationToken);

        if (group is null)
            return false;

        if (isActive)
            group.Activate();
        else
            group.Deactivate();

        await repository.SaveChangesAsync(cancellationToken);

        return true;
    }

    private static string NormalizeCode(string? code) =>
        code?.Trim().ToUpperInvariant() ?? string.Empty;

    private static string NormalizeName(string? name) =>
        name?.Trim() ?? string.Empty;

    private static void Validate(string code, string name)
    {
        if (string.IsNullOrWhiteSpace(code))
            throw new ArgumentException("Group code is required.");

        if (code.Length > 30)
            throw new ArgumentException(
                "Group code cannot exceed 30 characters.");

        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Group name is required.");

        if (name.Length > 150)
            throw new ArgumentException(
                "Group name cannot exceed 150 characters.");
    }

    private static GroupResponse MapToResponse(Group group) =>
        new(
            group.Id,
            group.Code,
            group.Name,
            group.IsActive,
            group.CreatedAtUtc);
}
