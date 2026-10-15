namespace Inventory.Application.DTOs;

public sealed record AdjustStockAtLocationRequest(
    Guid InventoryItemId,
    Guid StorageLocationId,
    decimal NewQuantity,
    string Remarks);
