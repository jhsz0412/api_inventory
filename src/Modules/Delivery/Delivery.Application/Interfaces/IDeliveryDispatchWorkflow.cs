using Delivery.Application.DTOs;

namespace Delivery.Application.Interfaces;

public interface IDeliveryDispatchWorkflow
{
    Task<DeliveryResponse> DispatchAsync(
        Guid deliveryId,
        Guid dispatchedByEmployeeId,
        CancellationToken cancellationToken = default);

    Task<bool> RecoverRequisitionFulfillmentAsync(
        Guid deliveryId,
        CancellationToken cancellationToken = default);
}