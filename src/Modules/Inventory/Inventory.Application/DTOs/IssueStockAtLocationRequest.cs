namespace Inventory.Application.DTOs;

public sealed record IssueStockAtLocationRequest(
    Guid InventoryItemId,
    Guid StorageLocationId,
    decimal Quantity,
    string? ReferenceNumber = null,
    string? Remarks = null);