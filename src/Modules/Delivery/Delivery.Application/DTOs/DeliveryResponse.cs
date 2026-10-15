
using Delivery.Domain.Enums;

namespace Delivery.Application.DTOs;

public sealed class DeliveryResponse
{
    public Guid Id { get; init; }

    public string DeliveryNumber { get; init; } = string.Empty;

    public Guid RequisitionId { get; init; }

    public DeliveryStatus Status { get; init; }

    public Guid? PreparedByEmployeeId { get; init; }

    public Guid? DispatchedByEmployeeId { get; init; }

    public DateTime CreatedAtUtc { get; init; }

    public DateTime? ReadyForDispatchAtUtc { get; init; }

    public DateTime? DispatchedAtUtc { get; init; }

    public string? Remarks { get; init; }

    public List<DeliveryItemResponse> Items { get; init; } = [];
}

public sealed class DeliveryItemResponse
{
    public Guid Id { get; init; }

    public Guid InventoryItemId { get; init; }

    public Guid StorageLocationId { get; init; }

    public decimal QuantityPrepared { get; init; }
}
