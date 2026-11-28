using Delivery.Application.DTOs;
using Delivery.Application.Interfaces;
using Delivery.Domain.Entities;
using Delivery.Domain.Enums;
using Inventory.Application.DTOs;
using Inventory.Application.Interfaces;
using Inventory.Application.Services;

namespace Delivery.Application.Services;

public sealed class DeliveryService : IDeliveryService
{
    private static readonly HashSet<string> DeliverableRequisitionStatuses =
        new(StringComparer.Ordinal)
        {
            "Approved",
            "FulfillmentInProgress"
        };

    private readonly IDeliveryRepository _repository;
    private readonly IInventoryService _inventoryService;
    private readonly IRequisitionDeliveryLookup _requisitionLookup;

    public DeliveryService(
        IDeliveryRepository repository,
        IInventoryService inventoryService,
        IRequisitionDeliveryLookup requisitionLookup)
    {
        _repository = repository;
        _inventoryService = inventoryService;
        _requisitionLookup = requisitionLookup;
    }

    public async Task<DeliveryResponse> CreateAsync(
        CreateDeliveryRequest request,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        if (request.Items is null || request.Items.Count == 0)
        {
            throw new ArgumentException(
                "A delivery must contain at least one item.",
                nameof(request));
        }

        var requisition = await _requisitionLookup.GetRequisitionAsync(
            request.RequisitionId,
            cancellationToken);

        if (requisition is null)
        {
            throw new KeyNotFoundException(
                $"Requisition '{request.RequisitionId}' was not found.");
        }

        if (!DeliverableRequisitionStatuses.Contains(requisition.Status))
        {
            throw new InvalidOperationException(
                $"A delivery can only be prepared for an approved requisition. " +
                $"Requisition '{requisition.RequisitionNumber}' is currently " +
                $"'{requisition.Status}'.");
        }

        foreach (var item in request.Items)
        {
            var inventoryItem = await _inventoryService.GetItemByIdAsync(
                item.InventoryItemId,
                cancellationToken);

            if (inventoryItem is null)
            {
                throw new KeyNotFoundException(
                    $"Inventory item '{item.InventoryItemId}' was not found.");
            }

            if (!inventoryItem.IsActive)
            {
                throw new InvalidOperationException(
                    $"Inventory item '{inventoryItem.ItemCode}' is inactive " +
                    "and cannot be prepared for delivery.");
            }
        }

        var deliveryNumber =
            $"DEL-{DateTime.UtcNow:yyyyMMddHHmmss}-{Guid.NewGuid():N}"[..32];

        var delivery = new Delivery.Domain.Entities.Delivery(
            deliveryNumber,
            request.RequisitionId,
            request.PreparedByEmployeeId,
            request.Remarks);

        foreach (var item in request.Items)
        {
            var deliveryItem = new DeliveryItem(
                item.InventoryItemId,
                item.StorageLocationId,
                item.QuantityPrepared);

            delivery.AddItem(deliveryItem);
        }

        await _repository.AddAsync(delivery, cancellationToken);
        await _repository.SaveChangesAsync(cancellationToken);

        return MapToResponse(delivery);
    }

    public async Task<DeliveryResponse?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        var delivery = await _repository.GetByIdAsync(
            id,
            cancellationToken);

        return delivery is null ? null : MapToResponse(delivery);
    }

    public async Task<IReadOnlyList<DeliveryResponse>> GetAllAsync(
        CancellationToken cancellationToken = default)
    {
        var deliveries = await _repository.GetAllAsync(cancellationToken);

        return deliveries.Select(MapToResponse).ToList();
    }

    public async Task<DeliveryResponse> MarkReadyForDispatchAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        var delivery = await GetDeliveryOrThrowAsync(
            id,
            cancellationToken);

        delivery.MarkReadyForDispatch();

        await _repository.SaveChangesAsync(cancellationToken);

        return MapToResponse(delivery);
    }

    public async Task<DeliveryResponse> DispatchAsync(
        Guid id,
        Guid dispatchedByEmployeeId,
        CancellationToken cancellationToken = default)
    {
        var delivery = await GetDeliveryOrThrowAsync(
            id,
            cancellationToken);

        if (dispatchedByEmployeeId == Guid.Empty)
        {
            throw new ArgumentException(
                "The dispatching employee ID is required.",
                nameof(dispatchedByEmployeeId));
        }

        if (delivery.Status != DeliveryStatus.ReadyForDispatch)
        {
            throw new InvalidOperationException(
                "Only deliveries ready for dispatch can be dispatched.");
        }

        var stockIssueRequest = new IssueStockAtLocationBatchRequest(
            ReferenceNumber: delivery.DeliveryNumber,
            Items: delivery.Items.Select(item =>
                new IssueStockAtLocationBatchItemRequest(
                    item.InventoryItemId,
                    item.StorageLocationId,
                    item.QuantityPrepared)).ToArray(),
            Remarks: $"Stock issued for delivery {delivery.DeliveryNumber}");

        // Deduct stock from the specified storage locations.
        await _inventoryService.IssueStockAtLocationBatchAsync(
            stockIssueRequest,
            cancellationToken);

        // Update delivery status only after the stock issue succeeds.
        delivery.Dispatch(dispatchedByEmployeeId);

        await _repository.SaveChangesAsync(cancellationToken);

        return MapToResponse(delivery);
    }

    public async Task CancelAsync(
        Guid id,
        string? remarks = null,
        CancellationToken cancellationToken = default)
    {
        var delivery = await GetDeliveryOrThrowAsync(
            id,
            cancellationToken);

        delivery.Cancel(remarks);

        await _repository.SaveChangesAsync(cancellationToken);
    }

    private async Task<Delivery.Domain.Entities.Delivery>
        GetDeliveryOrThrowAsync(
            Guid id,
            CancellationToken cancellationToken)
    {
        if (id == Guid.Empty)
        {
            throw new ArgumentException(
                "Delivery ID cannot be empty.",
                nameof(id));
        }

        return await _repository.GetByIdAsync(id, cancellationToken)
            ?? throw new KeyNotFoundException(
                $"Delivery '{id}' was not found.");
    }

    private static DeliveryResponse MapToResponse(
        Delivery.Domain.Entities.Delivery delivery)
    {
        return new DeliveryResponse
        {
            Id = delivery.Id,
            DeliveryNumber = delivery.DeliveryNumber,
            RequisitionId = delivery.RequisitionId,
            Status = delivery.Status,
            PreparedByEmployeeId = delivery.PreparedByEmployeeId,
            DispatchedByEmployeeId = delivery.DispatchedByEmployeeId,
            CreatedAtUtc = delivery.CreatedAtUtc,
            ReadyForDispatchAtUtc = delivery.ReadyForDispatchAtUtc,
            DispatchedAtUtc = delivery.DispatchedAtUtc,
            Remarks = delivery.Remarks,
            Items = delivery.Items.Select(item => new DeliveryItemResponse
            {
                Id = item.Id,
                InventoryItemId = item.InventoryItemId,
                StorageLocationId = item.StorageLocationId,
                QuantityPrepared = item.QuantityPrepared
            }).ToList()
        };
    }
}