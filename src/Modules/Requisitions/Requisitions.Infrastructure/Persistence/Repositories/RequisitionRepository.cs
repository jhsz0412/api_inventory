using Microsoft.EntityFrameworkCore;
using Requisitions.Application.Interfaces;
using Requisitions.Domain.Entities;
using Requisitions.Infrastructure.Persistence;
using System.Data;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace Requisitions.Infrastructure.Persistence.Repositories;

public sealed class RequisitionRepository : IRequisitionRepository
{
    private readonly RequisitionsDbContext _context;

    public RequisitionRepository(RequisitionsDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(
        Requisition requisition,
        CancellationToken cancellationToken = default)
    {
        await _context.Requisitions.AddAsync(
            requisition,
            cancellationToken);
    }

    public async Task AddItemAsync(
    RequisitionItem item,
    CancellationToken cancellationToken = default)
    {
        await _context.RequisitionItems.AddAsync(
            item,
            cancellationToken);
    }

    public async Task<Requisition?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        return await _context.Requisitions
            .Include(x => x.Items)
            .FirstOrDefaultAsync(
                x => x.Id == id,
                cancellationToken);
    }

    public async Task<IReadOnlyCollection<Requisition>> GetAllAsync(
        CancellationToken cancellationToken = default)
    {
        return await _context.Requisitions
            .Include(x => x.Items)
            .AsNoTracking()
            .OrderByDescending(x => x.RequestedAtUtc)
            .ToListAsync(cancellationToken);
    }

    public async Task<int> GetNextMonthlySequenceAsync(
        int year,
        int month,
        CancellationToken cancellationToken = default)
        {
            if (year < 1 || year > 9999)
                throw new ArgumentOutOfRangeException(nameof(year));

            if (month < 1 || month > 12)
                throw new ArgumentOutOfRangeException(nameof(month));

            await using var transaction =
                await _context.Database.BeginTransactionAsync(
                    IsolationLevel.Serializable,
                    cancellationToken);

            await _context.Database.ExecuteSqlInterpolatedAsync(
                $"""
            MERGE INTO dbo.RequisitionNumberSequences WITH (HOLDLOCK) AS target
            USING (VALUES ({year}, {month})) AS source ([Year], [Month])
            ON target.[Year] = source.[Year]
               AND target.[Month] = source.[Month]
            WHEN MATCHED THEN
                UPDATE SET [LastNumber] = target.[LastNumber] + 1
            WHEN NOT MATCHED THEN
                INSERT ([Year], [Month], [LastNumber])
                VALUES (source.[Year], source.[Month], 1);
            """,
                cancellationToken);

            var nextNumber = await _context.RequisitionNumberSequences
                .AsNoTracking()
                .Where(x => x.Year == year && x.Month == month)
                .Select(x => x.LastNumber)
                .SingleAsync(cancellationToken);

            await transaction.CommitAsync(cancellationToken);

            return nextNumber;
        }



    public async Task SaveChangesAsync(
        CancellationToken cancellationToken = default)
    {
        try
        {
            await _context.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateConcurrencyException exception)
        {
            throw new InvalidOperationException(
                "This requisition was changed by another request. " +
                "Please refresh the requisition and try again.",
                exception);
        }
    }


}