
using Microsoft.EntityFrameworkCore;
using Navigation.Application.DTOs;
using Navigation.Application.Interfaces;
using Navigation.Infrastructure.Persistence;
using Organization.Infrastructure.Persistence;

namespace InventoryManagement.Api.Integrations;

public sealed class GroupMenuVisibilityService(
    OrganizationDbContext organizationDbContext,
    NavigationDbContext navigationDbContext)
    : IGroupMenuVisibilityService
{
    public async Task<IReadOnlyList<MenuItemResponse>> GetVisibleMenuItemsAsync(
        Guid employeeId,
        CancellationToken cancellationToken = default)
    {
        if (employeeId == Guid.Empty)
        {
            return [];
        }

        // Find the employee's active groups.
        var groupIds = await (
            from member in organizationDbContext.GroupMembers
            join employeeGroup in organizationDbContext.Groups
                on member.GroupId equals employeeGroup.Id
            where member.EmployeeId == employeeId
                  && employeeGroup.IsActive
            select member.GroupId
        )
        .Distinct()
        .ToListAsync(cancellationToken);

        if (groupIds.Count == 0)
        {
            return [];
        }

        // Find menus assigned to any of the employee's active groups.
        var assignedMenuIds = await navigationDbContext.GroupMenuItems
            .AsNoTracking()
            .Where(assignment => groupIds.Contains(assignment.GroupId))
            .Select(assignment => assignment.MenuItemId)
            .Distinct()
            .ToListAsync(cancellationToken);

        if (assignedMenuIds.Count == 0)
        {
            return [];
        }

        // Retrieve ALL menu items, including active and inactive items.
        var allMenus = await navigationDbContext.MenuItems
            .AsNoTracking()
            .ToListAsync(cancellationToken);

        var menusById = allMenus.ToDictionary(menu => menu.Id);
        var visibleMenuIds = new HashSet<Guid>();

        // Include assigned menus and their ancestors,
        // regardless of their IsActive value.
        foreach (var menuId in assignedMenuIds)
        {
            if (!menusById.TryGetValue(menuId, out var currentMenu))
            {
                continue;
            }

            var visited = new HashSet<Guid>();

            while (true)
            {
                if (!visited.Add(currentMenu.Id))
                {
                    // Prevent an infinite loop if invalid hierarchy data exists.
                    break;
                }

                visibleMenuIds.Add(currentMenu.Id);

                if (currentMenu.ParentMenuItemId is not Guid parentId)
                {
                    break;
                }

                if (!menusById.TryGetValue(parentId, out var parentMenu))
                {
                    break;
                }

                currentMenu = parentMenu;
            }
        }

        return allMenus
            .Where(menu => visibleMenuIds.Contains(menu.Id))
            .OrderBy(menu => menu.SortOrder)
            .ThenBy(menu => menu.Name)
            .Select(menu => new MenuItemResponse(
                Id: menu.Id,
                Name: menu.Name,
                Label: menu.Label,
                Route: menu.Route,
                Icon: menu.Icon,
                ParentMenuItemId: menu.ParentMenuItemId,
                SortOrder: menu.SortOrder,
                IsActive: menu.IsActive,
                CreatedAtUtc: menu.CreatedAtUtc,
                UpdatedAtUtc: menu.UpdatedAtUtc))
            .ToList();
    }
}
