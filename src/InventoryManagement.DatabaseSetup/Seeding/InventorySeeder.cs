
using Inventory.Domain.Entities;
using Inventory.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace InventoryManagement.DatabaseSetup.Seeding;

public static class InventorySeeder
{
    public static async Task SeedAsync(
        InventoryDbContext inventoryDb,
        CancellationToken cancellationToken = default)
    {
        // 1. Create or reuse the inventory category.
        var category = await inventoryDb.InventoryCategories
            .SingleOrDefaultAsync(
                x => x.Code == "OFFICE",
                cancellationToken);

        if (category is null)
        {
            category = new InventoryCategory(
                "OFFICE",
                "Office Supplies",
                "General office supplies.");

            inventoryDb.InventoryCategories.Add(category);
            await inventoryDb.SaveChangesAsync(cancellationToken);
        }

        // 2. Create or reuse the unit of measure.
        var unit = await inventoryDb.UnitsOfMeasure
            .SingleOrDefaultAsync(
                x => x.Code == "PCS",
                cancellationToken);

        if (unit is null)
        {
            unit = new UnitOfMeasure("PCS", "Pieces");

            inventoryDb.UnitsOfMeasure.Add(unit);
            await inventoryDb.SaveChangesAsync(cancellationToken);
        }

        // 3. Create or reuse the inventory item.
        var item = await inventoryDb.InventoryItems
            .SingleOrDefaultAsync(
                x => x.ItemCode == "PEN-BLACK-001",
                cancellationToken);

        if (item is null)
        {
            item = new InventoryItem(
                itemCode: "PEN-BLACK-001",
                name: "Black Ballpoint Pen",
                categoryId: category.Id,
                unitOfMeasureId: unit.Id,
                description: "Standard black ballpoint pen.",
                reorderLevel: 10);

            inventoryDb.InventoryItems.Add(item);
            await inventoryDb.SaveChangesAsync(cancellationToken);
        }

        // 4. Create or reuse the warehouse.
        var warehouse = await inventoryDb.Warehouses
            .SingleOrDefaultAsync(
                x => x.Code == "MAIN",
                cancellationToken);

        if (warehouse is null)
        {
            warehouse = new Warehouse(
                "MAIN",
                "Main Warehouse",
                "Primary inventory storage facility.");

            inventoryDb.Warehouses.Add(warehouse);
            await inventoryDb.SaveChangesAsync(cancellationToken);
        }

        // 5. Create or reuse the storage location.
        var location = await inventoryDb.StorageLocations
            .SingleOrDefaultAsync(
                x => x.WarehouseId == warehouse.Id &&
                     x.Code == "A-01",
                cancellationToken);

        if (location is null)
        {
            location = new StorageLocation(
                warehouse.Id,
                "A-01",
                "A-01",
                "Primary storage location.");

            inventoryDb.StorageLocations.Add(location);
            await inventoryDb.SaveChangesAsync(cancellationToken);
        }

        Console.WriteLine(
            "Inventory defaults ready: OFFICE, PCS, " +
            "PEN-BLACK-001, MAIN, A-01.");
    }
}

