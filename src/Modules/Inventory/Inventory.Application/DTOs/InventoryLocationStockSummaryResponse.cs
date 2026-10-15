namespace Inventory.Application.DTOs;

public sealed record InventoryLocationStockSummaryResponse(
    Guid InventoryItemId,
    decimal TotalQuantityOnHand,
    int LocationCount,
    IReadOnlyCollection<InventoryLocationStockSummaryLocationResponse> Locations);

public sealed record InventoryLocationStockSummaryLocationResponse(
    Guid StorageLocationId,
    decimal QuantityOnHand);