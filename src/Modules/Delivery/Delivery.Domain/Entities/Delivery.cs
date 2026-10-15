
using Delivery.Domain.Enums;

namespace Delivery.Domain.Entities;

public sealed class Delivery
{
    private readonly List<DeliveryItem> _items = [];

    public Guid Id { get; private set; }

    public string DeliveryNumber { get; private set; } = string.Empty;

    public Guid RequisitionId { get; private set; }

    public DeliveryStatus Status { get; private set; }

    public Guid? PreparedByEmployeeId { get; private set; }

    public Guid? DispatchedByEmployeeId { get; private set; }

    public DateTime CreatedAtUtc { get; private set; }

    public DateTime? ReadyForDispatchAtUtc { get; private set; }

    public DateTime? DispatchedAtUtc { get; private set; }

    public string? Remarks { get; private set; }

    public IReadOnlyCollection<DeliveryItem> Items => _items.AsReadOnly();

    private Delivery()
    {
    }

    public Delivery(
        string deliveryNumber,
        Guid requisitionId,
        Guid preparedByEmployeeId,
        string? remarks = null)
    {
        if (string.IsNullOrWhiteSpace(deliveryNumber))
        {
            throw new ArgumentException(
                "Delivery number is required.",
                nameof(deliveryNumber));
        }

        if (deliveryNumber.Trim().Length > 50)
        {
            throw new ArgumentException(
                "Delivery number cannot exceed 50 characters.",
                nameof(deliveryNumber));
        }

        if (requisitionId == Guid.Empty)
        {
            throw new ArgumentException(
                "Requisition ID is required.",
                nameof(requisitionId));
        }

        if (preparedByEmployeeId == Guid.Empty)
        {
            throw new ArgumentException(
                "Preparing employee ID is required.",
                nameof(preparedByEmployeeId));
        }

        Id = Guid.NewGuid();
        DeliveryNumber = deliveryNumber.Trim();
        RequisitionId = requisitionId;
        PreparedByEmployeeId = preparedByEmployeeId;
        Status = DeliveryStatus.Preparing;
        CreatedAtUtc = DateTime.UtcNow;
        Remarks = NormalizeRemarks(remarks);
    }

    public void AddItem(DeliveryItem item)
    {
        ArgumentNullException.ThrowIfNull(item);

        if (Status != DeliveryStatus.Preparing)
        {
            throw new InvalidOperationException(
                "Items can only be added while the delivery is being prepared.");
        }

        if (_items.Any(x => x.InventoryItemId == item.InventoryItemId))
        {
            throw new InvalidOperationException(
                "An inventory item can only appear once in a delivery.");
        }

        item.AssignToDelivery(Id);
        _items.Add(item);
    }

    public void MarkReadyForDispatch()
    {
        if (Status != DeliveryStatus.Preparing)
        {
            throw new InvalidOperationException(
                "Only a delivery being prepared can be marked ready for dispatch.");
        }

        if (_items.Count == 0)
        {
            throw new InvalidOperationException(
                "A delivery must contain at least one item.");
        }

        ReadyForDispatchAtUtc = DateTime.UtcNow;
        Status = DeliveryStatus.ReadyForDispatch;
    }

    public void Dispatch(Guid dispatchedByEmployeeId)
    {
        if (Status != DeliveryStatus.ReadyForDispatch)
        {
            throw new InvalidOperationException(
                "Only a delivery ready for dispatch can be dispatched.");
        }

        if (dispatchedByEmployeeId == Guid.Empty)
        {
            throw new ArgumentException(
                "Dispatching employee ID is required.",
                nameof(dispatchedByEmployeeId));
        }

        DispatchedByEmployeeId = dispatchedByEmployeeId;
        DispatchedAtUtc = DateTime.UtcNow;
        Status = DeliveryStatus.Dispatched;
    }

    public void Cancel(string? remarks = null)
    {
        if (Status == DeliveryStatus.Dispatched)
        {
            throw new InvalidOperationException(
                "A dispatched delivery cannot be cancelled.");
        }

        if (Status == DeliveryStatus.Cancelled)
        {
            throw new InvalidOperationException(
                "The delivery is already cancelled.");
        }

        Status = DeliveryStatus.Cancelled;

        if (!string.IsNullOrWhiteSpace(remarks))
        {
            Remarks = NormalizeRemarks(remarks);
        }
    }

    private static string? NormalizeRemarks(string? remarks)
    {
        if (string.IsNullOrWhiteSpace(remarks))
        {
            return null;
        }

        var normalized = remarks.Trim();

        if (normalized.Length > 500)
        {
            throw new ArgumentException(
                "Remarks cannot exceed 500 characters.",
                nameof(remarks));
        }

        return normalized;
    }
}
