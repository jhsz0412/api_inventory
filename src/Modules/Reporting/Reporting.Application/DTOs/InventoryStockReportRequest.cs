namespace Reporting.Application.DTOs;

public sealed class InventoryStockReportRequest
{
    public Guid? InventoryItemId { get; init; }

    public Guid? WarehouseId { get; init; }

    public Guid? StorageLocationId { get; init; }
}