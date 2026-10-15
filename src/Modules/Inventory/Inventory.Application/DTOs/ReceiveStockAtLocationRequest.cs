namespace Inventory.Application.DTOs;

public sealed record ReceiveStockAtLocationRequest(
    Guid InventoryItemId,
    Guid StorageLocationId,
    decimal Quantity,
    string? ReferenceNumber = null,
    string? Remarks = null);