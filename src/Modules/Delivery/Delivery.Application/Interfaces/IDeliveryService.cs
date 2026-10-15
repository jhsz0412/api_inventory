
using Delivery.Application.DTOs;

namespace Delivery.Application.Interfaces;

public interface IDeliveryService
{
    Task<DeliveryResponse> CreateAsync(
        CreateDeliveryRequest request,
        CancellationToken cancellationToken = default);

    Task<DeliveryResponse?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<DeliveryResponse>> GetAllAsync(
        CancellationToken cancellationToken = default);

    Task<DeliveryResponse> MarkReadyForDispatchAsync(
        Guid id,
        CancellationToken cancellationToken = default);

    Task<DeliveryResponse> DispatchAsync(
        Guid id,
        Guid dispatchedByEmployeeId,
        CancellationToken cancellationToken = default);

    Task CancelAsync(
        Guid id,
        string? remarks = null,
        CancellationToken cancellationToken = default);
}
