
using Delivery.Application.Interfaces;
using Delivery.Infrastructure.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;
using DeliveryEntity = Delivery.Domain.Entities.Delivery;

namespace Delivery.Infrastructure.Persistence.Repositories;

public sealed class DeliveryRepository : IDeliveryRepository
{
    private readonly DeliveryDbContext _context;

    public DeliveryRepository(DeliveryDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(
        DeliveryEntity delivery,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(delivery);

        await _context.Deliveries.AddAsync(
            delivery,
            cancellationToken);
    }

    public async Task<DeliveryEntity?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        return await _context.Deliveries
            .Include(x => x.Items)
            .FirstOrDefaultAsync(
                x => x.Id == id,
                cancellationToken);
    }

    public async Task<IReadOnlyList<DeliveryEntity>> GetAllAsync(
        CancellationToken cancellationToken = default)
    {
        return await _context.Deliveries
            .Include(x => x.Items)
            .OrderByDescending(x => x.CreatedAtUtc)
            .ToListAsync(cancellationToken);
    }

    public async Task<bool> DeliveryNumberExistsAsync(
        string deliveryNumber,
        CancellationToken cancellationToken = default)
    {
        return await _context.Deliveries
            .AnyAsync(
                x => x.DeliveryNumber == deliveryNumber,
                cancellationToken);
    }

    public async Task SaveChangesAsync(
        CancellationToken cancellationToken = default)
    {
        await _context.SaveChangesAsync(cancellationToken);
    }
}
