namespace Reporting.Domain.Entities;

public sealed class DeliveryReport
{
    public Guid DeliveryId { get; private set; }

    public string DeliveryNumber { get; private set; }

    public Guid RequisitionId { get; private set; }

    public string Status { get; private set; }

    public Guid? PreparedByEmployeeId { get; private set; }

    public Guid? DispatchedByEmployeeId { get; private set; }

    public DateTime CreatedAtUtc { get; private set; }

    public DateTime? ReadyForDispatchAtUtc { get; private set; }

    public DateTime? DispatchedAtUtc { get; private set; }

    public string? Remarks { get; private set; }

    public Guid DeliveryItemId { get; private set; }

    public Guid InventoryItemId { get; private set; }

    public Guid StorageLocationId { get; private set; }

    public decimal QuantityPrepared { get; private set; }

    public DeliveryReport(
        Guid deliveryId,
        string deliveryNumber,
        Guid requisitionId,
        string status,
        Guid? preparedByEmployeeId,
        Guid? dispatchedByEmployeeId,
        DateTime createdAtUtc,
        DateTime? readyForDispatchAtUtc,
        DateTime? dispatchedAtUtc,
        string? remarks,
        Guid deliveryItemId,
        Guid inventoryItemId,
        Guid storageLocationId,
        decimal quantityPrepared)
    {
        DeliveryId = deliveryId;
        DeliveryNumber = deliveryNumber;
        RequisitionId = requisitionId;
        Status = status;
        PreparedByEmployeeId = preparedByEmployeeId;
        DispatchedByEmployeeId = dispatchedByEmployeeId;
        CreatedAtUtc = createdAtUtc;
        ReadyForDispatchAtUtc = readyForDispatchAtUtc;
        DispatchedAtUtc = dispatchedAtUtc;
        Remarks = remarks;
        DeliveryItemId = deliveryItemId;
        InventoryItemId = inventoryItemId;
        StorageLocationId = storageLocationId;
        QuantityPrepared = quantityPrepared;
    }
}