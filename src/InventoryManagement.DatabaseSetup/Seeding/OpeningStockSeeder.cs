
using Inventory.Application.DTOs;
using Inventory.Application.Services;
using Inventory.Domain.Entities;
using Inventory.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace InventoryManagement.DatabaseSetup.Seeding;

public static class OpeningStockSeeder
{
    private const string ItemCode = "PEN-BLACK-001";
    private const string WarehouseCode = "MAIN";
    private const string LocationCode = "A-01";
    private const decimal OpeningQuantity = 100m;

    private const string ReferenceNumber =
        "SEED-OPENING-PEN-BLACK-001-2026";

    public static async Task SeedAsync(
        InventoryDbContext inventoryDb,
        IInventoryService inventoryService,
        CancellationToken cancellationToken = default)
    {
        var item = await inventoryDb.InventoryItems
            .SingleOrDefaultAsync(
                x => x.ItemCode == ItemCode,
                cancellationToken)
            ?? throw new InvalidOperationException(
                $"Inventory item '{ItemCode}' was not found. " +
                "Run InventorySeeder first.");

        var warehouse = await inventoryDb.Warehouses
            .SingleOrDefaultAsync(
                x => x.Code == WarehouseCode,
                cancellationToken)
            ?? throw new InvalidOperationException(
                $"Warehouse '{WarehouseCode}' was not found.");

        var location = await inventoryDb.StorageLocations
            .SingleOrDefaultAsync(
                x => x.WarehouseId == warehouse.Id &&
                     x.Code == LocationCode,
                cancellationToken)
            ?? throw new InvalidOperationException(
                $"Storage location '{LocationCode}' was not found.");

        // Look up the fixed reference before receiving any stock.
        var existingTransactions = await inventoryDb
            .InventoryStockTransactions
            .Where(x => x.ReferenceNumber == ReferenceNumber)
            .ToListAsync(cancellationToken);

        if (existingTransactions.Count > 0)
        {
            var matchingTransaction = existingTransactions.Any(
                x => x.InventoryItemId == item.Id &&
                     x.StorageLocationId == location.Id &&
                     x.TransactionType ==
                         InventoryStockTransactionType.StockIn);

            if (!matchingTransaction)
            {
                throw new InvalidOperationException(
                    $"Opening-stock reference '{ReferenceNumber}' " +
                    "already exists for a different transaction. " +
                    "No stock was received.");
            }

            Console.WriteLine(
                $"Opening stock already processed. " +
                $"Reference: {ReferenceNumber}. No stock added.");

            return;
        }

        if (!location.IsActive)
        {
            throw new InvalidOperationException(
                $"Storage location '{LocationCode}' is inactive. " +
                "No stock was received.");
        }

        await inventoryService.ReceiveStockAtLocationAsync(
            new ReceiveStockAtLocationRequest(
                InventoryItemId: item.Id,
                StorageLocationId: location.Id,
                Quantity: OpeningQuantity,
                ReferenceNumber: ReferenceNumber,
                Remarks: "Default opening stock seeded by DatabaseSetup."),
            cancellationToken);

        Console.WriteLine(
            $"Opening stock received: {OpeningQuantity} pieces of " +
            $"{ItemCode} at {WarehouseCode}/{LocationCode}. " +
            $"Reference: {ReferenceNumber}");
    }
}
