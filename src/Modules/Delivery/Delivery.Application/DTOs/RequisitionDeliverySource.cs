namespace Delivery.Application.DTOs;

public sealed record RequisitionDeliverySource(
    Guid RequisitionId,
    string RequisitionNumber,
    string Status,
    IReadOnlyCollection<RequisitionDeliveryItemSource> Items);

public sealed record RequisitionDeliveryItemSource(
    Guid InventoryItemId,
    int QuantityRequested);
