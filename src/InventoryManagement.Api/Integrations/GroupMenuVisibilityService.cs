
using InventoryManagement.Api.Integrations;
using Microsoft.EntityFrameworkCore;
using Navigation.Application.DTOs;
using Navigation.Infrastructure.Persistence;
using Organization.Infrastructure.Persistence;
using Navigation.Application.Interfaces;

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

        // Find menus explicitly assigned to any of those groups.
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

        var activeMenus = await navigationDbContext.MenuItems
            .AsNoTracking()
            .Where(menu => menu.IsActive)
            .ToListAsync(cancellationToken);

        var menusById = activeMenus.ToDictionary(menu => menu.Id);
        var visibleMenuIds = new HashSet<Guid>();

        // Include assigned active menus and their active ancestors.
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
                    // Avoid an infinite loop if bad hierarchy data exists.
                    break;
                }

                visibleMenuIds.Add(currentMenu.Id);

                if (currentMenu.ParentMenuItemId is not Guid parentId)
                {
                    break;
                }

                if (!menusById.TryGetValue(parentId, out var parentMenu))
                {
                    // Do not return a child whose parent is inactive or missing.
                    visibleMenuIds.Remove(currentMenu.Id);
                    break;
                }

                currentMenu = parentMenu;
            }
        }

        return activeMenus
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
