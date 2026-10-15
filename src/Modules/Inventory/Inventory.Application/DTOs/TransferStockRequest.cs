namespace Inventory.Application.DTOs;

public sealed record TransferStockRequest(
    Guid InventoryItemId,
    Guid SourceStorageLocationId,
    Guid DestinationStorageLocationId,
    decimal Quantity,
    string? ReferenceNumber = null,
    string? Remarks = null);