namespace Requisitions.Application.DTOs;

public sealed record RequisitionStockIssueRequest(
    Guid InventoryItemId,
    decimal Quantity);