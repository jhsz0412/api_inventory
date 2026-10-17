
using Navigation.Application.DTOs;

namespace Navigation.Application.Services;

public interface IMenuItemService
{
    Task<MenuItemResponse> CreateAsync(
        CreateMenuItemRequest request,
        CancellationToken cancellationToken = default);

    Task<MenuItemResponse?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<MenuItemResponse>> GetAllAsync(
        CancellationToken cancellationToken = default);

    Task<MenuItemResponse?> UpdateAsync(
        Guid id,
        UpdateMenuItemRequest request,
        CancellationToken cancellationToken = default);

    Task<bool> DeactivateAsync(
        Guid id,
        CancellationToken cancellationToken = default);
}
