
using Navigation.Application.DTOs;

namespace Navigation.Application.Interfaces;

public interface IGroupMenuAssignmentService
{
    Task<IReadOnlyList<MenuItemResponse>> GetAssignedMenuItemsAsync(
        Guid groupId,
        CancellationToken cancellationToken = default);

    Task AssignMenuAsync(
        Guid groupId,
        Guid menuItemId,
        CancellationToken cancellationToken = default);

    Task<bool> RemoveMenuAsync(
        Guid groupId,
        Guid menuItemId,
        CancellationToken cancellationToken = default);
}
