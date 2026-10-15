namespace Inventory.Application.DTOs;

public sealed record IssueInventoryStockRequest(
    Guid InventoryItemId,
    decimal Quantity);