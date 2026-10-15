using Requisitions.Domain.Entities;

namespace Requisitions.Application.Interfaces;

public interface IRequisitionRepository
{
    Task AddAsync(
        Requisition requisition,
        CancellationToken cancellationToken = default);

    Task<Requisition?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyCollection<Requisition>> GetAllAsync(
        CancellationToken cancellationToken = default);

    Task<int> GetNextMonthlySequenceAsync(
        int year,
        int month,
        CancellationToken cancellationToken = default);

    Task SaveChangesAsync(
        CancellationToken cancellationToken = default);

    Task AddItemAsync(
    RequisitionItem item,
        CancellationToken cancellationToken = default);
}