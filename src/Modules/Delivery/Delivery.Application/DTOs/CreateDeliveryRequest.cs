
namespace Delivery.Application.DTOs;

public sealed class CreateDeliveryRequest
{
    public Guid RequisitionId { get; init; }

    public Guid PreparedByEmployeeId { get; init; }

    public string? Remarks { get; init; }

    public List<CreateDeliveryItemRequest> Items { get; init; } = [];
}

public sealed class CreateDeliveryItemRequest
{
    public Guid InventoryItemId { get; init; }

    public Guid StorageLocationId { get; init; }

    public decimal QuantityPrepared { get; init; }
}
