
using Navigation.Application.DTOs;

namespace Navigation.Application.Interfaces;

public interface IGroupMenuVisibilityService
{
    Task<IReadOnlyList<MenuItemResponse>> GetVisibleMenuItemsAsync(
        Guid employeeId,
        CancellationToken cancellationToken = default);
}
