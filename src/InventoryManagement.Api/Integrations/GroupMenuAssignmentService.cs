
using Microsoft.EntityFrameworkCore;
using Navigation.Application.DTOs;
using Navigation.Application.Interfaces;
using Navigation.Domain.Entities;
using Navigation.Infrastructure.Persistence;
using Organization.Infrastructure.Persistence;

namespace InventoryManagement.Api.Integrations;

public sealed class GroupMenuAssignmentService(
    OrganizationDbContext organizationDbContext,
    NavigationDbContext navigationDbContext)
    : IGroupMenuAssignmentService
{
    public async Task<IReadOnlyList<MenuItemResponse>> GetAssignedMenuItemsAsync(
        Guid groupId,
        CancellationToken cancellationToken = default)
    {
        await EnsureGroupExistsAsync(groupId, cancellationToken);

        var assignedMenuIds = await navigationDbContext.GroupMenuItems
            .AsNoTracking()
            .Where(assignment => assignment.GroupId == groupId)
            .Select(assignment => assignment.MenuItemId)
            .ToListAsync(cancellationToken);

        var menuItems = await navigationDbContext.MenuItems
            .AsNoTracking()
            .Where(menu => assignedMenuIds.Contains(menu.Id))
            .OrderBy(menu => menu.SortOrder)
            .ThenBy(menu => menu.Name)
            .Select(menu => new MenuItemResponse(
                menu.Id,
                menu.Name,
                menu.Label,
                menu.Route,
                menu.Icon,
                menu.ParentMenuItemId,
                menu.SortOrder,
                menu.IsActive,
                menu.CreatedAtUtc,
                menu.UpdatedAtUtc))
            .ToListAsync(cancellationToken);

        return menuItems;
    }

    public async Task AssignMenuAsync(
        Guid groupId,
        Guid menuItemId,
        CancellationToken cancellationToken = default)
    {
        await EnsureGroupExistsAsync(groupId, cancellationToken);

        if (menuItemId == Guid.Empty)
        {
            throw new ArgumentException(
                "A valid menu item is required.",
                nameof(menuItemId));
        }

        var menuExists = await navigationDbContext.MenuItems
            .AnyAsync(menu => menu.Id == menuItemId, cancellationToken);

        if (!menuExists)
        {
            throw new KeyNotFoundException("The menu item was not found.");
        }

        var alreadyAssigned = await navigationDbContext.GroupMenuItems
            .AnyAsync(
                assignment => assignment.GroupId == groupId
                    && assignment.MenuItemId == menuItemId,
                cancellationToken);

        if (alreadyAssigned)
        {
            throw new InvalidOperationException(
                "This menu item is already assigned to the group.");
        }

        navigationDbContext.GroupMenuItems.Add(
            new GroupMenuItem(groupId, menuItemId));

        await navigationDbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<bool> RemoveMenuAsync(
        Guid groupId,
        Guid menuItemId,
        CancellationToken cancellationToken = default)
    {
        await EnsureGroupExistsAsync(groupId, cancellationToken);

        var assignment = await navigationDbContext.GroupMenuItems
            .FirstOrDefaultAsync(
                item => item.GroupId == groupId
                    && item.MenuItemId == menuItemId,
                cancellationToken);

        if (assignment is null)
        {
            return false;
        }

        navigationDbContext.GroupMenuItems.Remove(assignment);

        await navigationDbContext.SaveChangesAsync(cancellationToken);

        return true;
    }

    private async Task EnsureGroupExistsAsync(
        Guid groupId,
        CancellationToken cancellationToken)
    {
        if (groupId == Guid.Empty)
        {
            throw new ArgumentException(
                "A valid group is required.",
                nameof(groupId));
        }

        var groupExists = await organizationDbContext.Groups
            .AnyAsync(
                group => group.Id == groupId && group.IsActive,
                cancellationToken);

        if (!groupExists)
        {
            throw new KeyNotFoundException(
                "The active group was not found.");
        }
    }
}
