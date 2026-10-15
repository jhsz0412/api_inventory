namespace Inventory.Application.DTOs;

public sealed record IssueStockAtLocationBatchRequest(
    string ReferenceNumber,
    IReadOnlyCollection<IssueStockAtLocationBatchItemRequest> Items,
    string? Remarks = null);

public sealed record IssueStockAtLocationBatchItemRequest(
    Guid InventoryItemId,
    Guid StorageLocationId,
    decimal Quantity);
