
using Identity.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Navigation.Domain.Entities;
using Navigation.Infrastructure.Persistence;

namespace InventoryManagement.DatabaseSetup.Seeding;

public static class NavigationSeeder
{
    public static async Task SeedAsync(
        NavigationDbContext navigationDb,
        IdentityDbContext identityDb,
        CancellationToken cancellationToken = default)
    {
        // Resolve existing Identity permissions by code.
        var permissionCodes = new[]
        {
            "inventory.items.read",
            "inventory.categories.read",
            "inventory.units.read",
            "inventory.warehouses.read",
            "inventory.storage-locations.read",
            "inventory.stock.read"
        };

        var permissions = await identityDb.Permissions
            .Where(p => permissionCodes.Contains(p.Code))
            .ToDictionaryAsync(
                p => p.Code,
                p => p.Id,
                cancellationToken);

        // Create or reuse the Inventory parent menu.
        var inventory = await navigationDb.MenuItems
            .SingleOrDefaultAsync(
                m => m.Name == "Inventory",
                cancellationToken);

        if (inventory is null)
        {
            inventory = new MenuItem(
                name: "Inventory",
                label: "Inventory",
                route: null,
                icon: "boxes",
                parentMenuItemId: null,
                sortOrder: 1);

            navigationDb.MenuItems.Add(inventory);
            await navigationDb.SaveChangesAsync(cancellationToken);
        }

        // Child menu definitions use existing permission codes.
        var children = new[]
        {
            new
            {
                Name = "InventoryItems",
                Label = "Items",
                Route = "/inventory/items",
                Icon = "package",
                PermissionCode = "inventory.items.read",
                SortOrder = 1
            },
            new
            {
                Name = "InventoryCategories",
                Label = "Categories",
                Route = "/inventory/categories",
                Icon = "tags",
                PermissionCode = "inventory.categories.read",
                SortOrder = 2
            },
            new
            {
                Name = "UnitsOfMeasurement",
                Label = "Units of Measurement",
                Route = "/inventory/units-of-measure",
                Icon = "ruler",
                PermissionCode = "inventory.units.read",
                SortOrder = 3
            },
            new
            {
                Name = "Warehouses",
                Label = "Warehouses",
                Route = "/inventory/warehouses",
                Icon = "warehouse",
                PermissionCode = "inventory.warehouses.read",
                SortOrder = 4
            },
            new
            {
                Name = "StorageLocations",
                Label = "Storage Locations",
                Route = "/inventory/storage-locations",
                Icon = "map-pin",
                PermissionCode = "inventory.storage-locations.read",
                SortOrder = 5
            },
            new
            {
                Name = "InventoryStock",
                Label = "Stock",
                Route = "/inventory/stock",
                Icon = "boxes",
                PermissionCode = "inventory.stock.read",
                SortOrder = 6
            },
            new
            {
                Name = "StockMovements",
                Label = "Stock Movements",
                Route = "/inventory/stock-movements",
                Icon = "arrow-left-right",
                PermissionCode = "inventory.stock.read",
                SortOrder = 7
            }
        };

        foreach (var definition in children)
        {
            // Reuse the menu if it already exists.
            var existing = await navigationDb.MenuItems
                .SingleOrDefaultAsync(
                    m => m.Name == definition.Name,
                    cancellationToken);

            if (existing is not null)
            {
                continue;
            }

            // Do not create a menu with a nonexistent permission.
            if (!permissions.TryGetValue(
                    definition.PermissionCode,
                    out var permissionId))
            {
                throw new InvalidOperationException(
                    $"Permission '{definition.PermissionCode}' " +
                    "was not found. Run IdentitySeeder first.");
            }

            var child = new MenuItem(
                name: definition.Name,
                label: definition.Label,
                route: definition.Route,
                icon: definition.Icon,
                parentMenuItemId: inventory.Id,
                sortOrder: definition.SortOrder);

            navigationDb.MenuItems.Add(child);
        }

        await navigationDb.SaveChangesAsync(cancellationToken);

        Console.WriteLine(
            "Navigation defaults ready: Inventory and its child menus.");
    }
}
