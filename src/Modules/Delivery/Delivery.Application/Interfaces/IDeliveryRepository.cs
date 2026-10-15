
using DeliveryEntity = Delivery.Domain.Entities.Delivery;

namespace Delivery.Application.Interfaces;

public interface IDeliveryRepository
{
    Task AddAsync(
        DeliveryEntity delivery,
        CancellationToken cancellationToken = default);

    Task<DeliveryEntity?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<DeliveryEntity>> GetAllAsync(
        CancellationToken cancellationToken = default);

    Task<bool> DeliveryNumberExistsAsync(
        string deliveryNumber,
        CancellationToken cancellationToken = default);

    Task SaveChangesAsync(
        CancellationToken cancellationToken = default);
}
