
using Navigation.Application.DTOs;
using Navigation.Application.Interfaces;
using Navigation.Domain.Entities;

namespace Navigation.Application.Services;

public sealed class MenuItemService(
    IMenuItemRepository repository) : IMenuItemService
{
    public async Task<MenuItemResponse> CreateAsync(
        CreateMenuItemRequest request,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        ValidateRequiredText(request.Name, nameof(request.Name));
        ValidateRequiredText(request.Label, nameof(request.Label));
        ValidateSortOrder(request.SortOrder);

        var name = request.Name.Trim();

        if (await repository.ExistsByNameAsync(
                name,
                cancellationToken: cancellationToken))
        {
            throw new InvalidOperationException(
                $"A menu item with the name '{name}' already exists.");
        }

        if (request.ParentMenuItemId.HasValue &&
            !await repository.ParentExistsAsync(
                request.ParentMenuItemId.Value,
                cancellationToken))
        {
            throw new InvalidOperationException(
                "The specified active parent menu item was not found.");
        }

        var menuItem = new MenuItem(
            name: name,
            label: request.Label,
            route: request.Route,
            icon: request.Icon,
            parentMenuItemId: request.ParentMenuItemId,
            sortOrder: request.SortOrder);

        await repository.AddAsync(menuItem, cancellationToken);
        await repository.SaveChangesAsync(cancellationToken);

        return MapToResponse(menuItem);
    }

    public async Task<MenuItemResponse?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        var menuItem = await repository.GetByIdAsync(
            id,
            cancellationToken);

        return menuItem is null ? null : MapToResponse(menuItem);
    }

    public async Task<IReadOnlyList<MenuItemResponse>> GetAllAsync(
        CancellationToken cancellationToken = default)
    {
        var menuItems = await repository.GetAllAsync(cancellationToken);

        return menuItems
            .Where(menuItem => menuItem.IsActive)
            .Select(MapToResponse)
            .ToList();
    }

    public async Task<MenuItemResponse?> UpdateAsync(
        Guid id,
        UpdateMenuItemRequest request,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        var menuItem = await repository.GetByIdAsync(
            id,
            cancellationToken);

        if (menuItem is null)
        {
            return null;
        }

        ValidateRequiredText(request.Name, nameof(request.Name));
        ValidateRequiredText(request.Label, nameof(request.Label));
        ValidateSortOrder(request.SortOrder);

        var name = request.Name.Trim();

        if (await repository.ExistsByNameAsync(
                name,
                excludingId: id,
                cancellationToken: cancellationToken))
        {
            throw new InvalidOperationException(
                $"A menu item with the name '{name}' already exists.");
        }

        if (request.ParentMenuItemId.HasValue)
        {
            var parentId = request.ParentMenuItemId.Value;

            if (parentId == id)
            {
                throw new InvalidOperationException(
                    "A menu item cannot be its own parent.");
            }

            if (!await repository.ParentExistsAsync(
                    parentId,
                    cancellationToken))
            {
                throw new InvalidOperationException(
                    "The specified active parent menu item was not found.");
            }

            await EnsureNoCircularRelationshipAsync(
                id,
                parentId,
                cancellationToken);
        }

        menuItem.Update(
            name: name,
            label: request.Label,
            route: request.Route,
            icon: request.Icon,
            parentMenuItemId: request.ParentMenuItemId,
            sortOrder: request.SortOrder);

        repository.Update(menuItem);
        await repository.SaveChangesAsync(cancellationToken);

        return MapToResponse(menuItem);
    }

    public async Task<bool> DeactivateAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        var menuItem = await repository.GetByIdAsync(
            id,
            cancellationToken);

        if (menuItem is null)
        {
            return false;
        }

        menuItem.Deactivate();

        repository.Update(menuItem);
        await repository.SaveChangesAsync(cancellationToken);

        return true;
    }

    private async Task EnsureNoCircularRelationshipAsync(
        Guid menuItemId,
        Guid proposedParentId,
        CancellationToken cancellationToken)
    {
        var visited = new HashSet<Guid>();
        var currentId = proposedParentId;

        while (true)
        {
            if (currentId == menuItemId)
            {
                throw new InvalidOperationException(
                    "This parent assignment would create a circular menu relationship.");
            }

            if (!visited.Add(currentId))
            {
                throw new InvalidOperationException(
                    "The existing menu hierarchy contains a circular relationship.");
            }

            var current = await repository.GetByIdAsync(
                currentId,
                cancellationToken);

            if (current?.ParentMenuItemId is not Guid nextParentId)
            {
                return;
            }

            currentId = nextParentId;
        }
    }

    private static void ValidateRequiredText(
        string value,
        string parameterName)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(
            value,
            parameterName);
    }

    private static void ValidateSortOrder(int sortOrder)
    {
        if (sortOrder < 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(sortOrder),
                "Sort order cannot be negative.");
        }
    }

    private static MenuItemResponse MapToResponse(MenuItem menuItem)
    {
        return new MenuItemResponse(
            Id: menuItem.Id,
            Name: menuItem.Name,
            Label: menuItem.Label,
            Route: menuItem.Route,
            Icon: menuItem.Icon,
            ParentMenuItemId: menuItem.ParentMenuItemId,
            SortOrder: menuItem.SortOrder,
            IsActive: menuItem.IsActive,
            CreatedAtUtc: menuItem.CreatedAtUtc,
            UpdatedAtUtc: menuItem.UpdatedAtUtc);
    }
}
