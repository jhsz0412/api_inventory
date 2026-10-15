using Delivery.Application.DTOs;
using Delivery.Application.Interfaces;
using Requisitions.Application.Interfaces;
using Delivery.Domain.Enums;

namespace InventoryManagement.Api.Integrations;

public sealed class DeliveryDispatchService : IDeliveryDispatchWorkflow
{
    private readonly IDeliveryService _deliveryService;
    private readonly IRequisitionService _requisitionService;

    public DeliveryDispatchService(
        IDeliveryService deliveryService,
        IRequisitionService requisitionService)
    {
        _deliveryService = deliveryService;
        _requisitionService = requisitionService;
    }

    public async Task<DeliveryResponse> DispatchAsync(
        Guid deliveryId,
        Guid dispatchedByEmployeeId,
        CancellationToken cancellationToken = default)
    {
        var delivery = await _deliveryService.DispatchAsync(
            deliveryId,
            dispatchedByEmployeeId,
            cancellationToken);

        var requisition =
            await _requisitionService.MarkFulfilledAfterDeliveryAsync(
                delivery.RequisitionId,
                cancellationToken);

        if (requisition is null)
        {
            throw new InvalidOperationException(
                $"Delivery {delivery.Id} was dispatched, but requisition " +
                $"{delivery.RequisitionId} was not found. Requisition status " +
                "requires recovery.");
        }

        return delivery;
    }

    public async Task<bool> RecoverRequisitionFulfillmentAsync(
        Guid deliveryId,
        CancellationToken cancellationToken = default)
    {
        var delivery = await _deliveryService.GetByIdAsync(
            deliveryId,
            cancellationToken);

        if (delivery is null)
        {
            throw new KeyNotFoundException(
                $"Delivery {deliveryId} was not found.");
        }

        if (delivery.Status != DeliveryStatus.Dispatched)
        {
            throw new InvalidOperationException(
                "Only dispatched deliveries can recover requisition fulfillment.");
        }

        var requisition =
            await _requisitionService.MarkFulfilledAfterDeliveryAsync(
                delivery.RequisitionId,
                cancellationToken);

        return requisition is not null;
    }
}