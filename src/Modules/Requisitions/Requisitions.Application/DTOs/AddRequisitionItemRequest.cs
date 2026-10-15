namespace Requisitions.Application.DTOs;

public sealed record AddRequisitionItemRequest(
    Guid InventoryItemId,
    int QuantityRequested,
    string? Notes);