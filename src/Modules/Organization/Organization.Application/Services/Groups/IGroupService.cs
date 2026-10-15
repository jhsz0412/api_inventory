using Organization.Application.DTOs.Groups;

namespace Organization.Application.Services.Groups;

public interface IGroupService
{
    Task<IReadOnlyList<GroupResponse>> GetAllAsync(
        CancellationToken cancellationToken = default);

    Task<GroupResponse?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default);

    Task<GroupResponse> CreateAsync(
        CreateGroupRequest request,
        CancellationToken cancellationToken = default);

    Task<GroupResponse?> UpdateAsync(
        Guid id,
        UpdateGroupRequest request,
        CancellationToken cancellationToken = default);

    Task<bool> SetStatusAsync(
        Guid id,
        bool isActive,
        CancellationToken cancellationToken = default);
}