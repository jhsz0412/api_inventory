using Delivery.Domain.Enums;
using Delivery.Infrastructure.Persistence;
using Inventory.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Reporting.Application.DTOs;
using Reporting.Application.Interfaces;
using Reporting.Domain.Entities;
using Requisitions.Domain.Enums;
using Requisitions.Infrastructure.Persistence;

namespace Reporting.Infrastructure.Persistence.Repositories;

public sealed class ReportingRepository : IReportingRepository
{
    private readonly InventoryDbContext _context;
    private readonly RequisitionsDbContext _requisitionsContext;
    private readonly DeliveryDbContext _deliveryContext;

    public ReportingRepository(InventoryDbContext context, RequisitionsDbContext requisitionsContext, DeliveryDbContext deliveryContext)
    {
        _context = context;
        _requisitionsContext = requisitionsContext;
        _deliveryContext = deliveryContext;
    }

    public async Task<IReadOnlyCollection<InventoryStockReport>>
        GetInventoryStockReportAsync(
            Guid? inventoryItemId = null,
            Guid? warehouseId = null,
            Guid? storageLocationId = null,
            CancellationToken cancellationToken = default)
    {
        var query =
            from balance in _context.InventoryLocationStockBalances.AsNoTracking()
            join item in _context.InventoryItems.AsNoTracking()
                on balance.InventoryItemId equals item.Id
            join location in _context.StorageLocations.AsNoTracking()
                on balance.StorageLocationId equals location.Id
            join warehouse in _context.Warehouses.AsNoTracking()
                on location.WarehouseId equals warehouse.Id
            select new
            {
                Balance = balance,
                Item = item,
                Location = location,
                Warehouse = warehouse
            };

        if (inventoryItemId.HasValue)
        {
            query = query.Where(x =>
                x.Item.Id == inventoryItemId.Value);
        }

        if (warehouseId.HasValue)
        {
            query = query.Where(x =>
                x.Warehouse.Id == warehouseId.Value);
        }

        if (storageLocationId.HasValue)
        {
            query = query.Where(x =>
                x.Location.Id == storageLocationId.Value);
        }

        var results = await query
            .OrderBy(x => x.Warehouse.Name)
            .ThenBy(x => x.Location.Code)
            .ThenBy(x => x.Item.Name)
            .Select(x => new InventoryStockReport(
                x.Item.Id,
                x.Item.ItemCode,
                x.Item.Name,
                x.Warehouse.Id,
                x.Warehouse.Name,
                x.Location.Id,
                x.Location.Code,
                x.Balance.QuantityOnHand,
                x.Balance.LastUpdatedUtc))
            .ToListAsync(cancellationToken);

        return results;
    }

    public async Task<IReadOnlyCollection<LowStockReport>>
        GetLowStockReportAsync(
            CancellationToken cancellationToken = default)
        {
            var results = await (
                from balance in _context.InventoryStockBalances.AsNoTracking()
                join item in _context.InventoryItems.AsNoTracking()
                    on balance.InventoryItemId equals item.Id
                where balance.QuantityOnHand <= item.ReorderLevel
                orderby balance.QuantityOnHand, item.Name
                select new LowStockReport(
                    item.Id,
                    item.ItemCode,
                    item.Name,
                    balance.QuantityOnHand,
                    item.ReorderLevel,
                    balance.LastUpdatedUtc)
            ).ToListAsync(cancellationToken);

            return results;
    }

    public async Task<IReadOnlyCollection<StockMovementReport>>
        GetStockMovementReportAsync(
            Guid? inventoryItemId = null,
            DateTime? fromUtc = null,
            DateTime? toUtc = null,
            CancellationToken cancellationToken = default)
        {
            var query =
                from transaction in _context.InventoryStockTransactions.AsNoTracking()
                join item in _context.InventoryItems.AsNoTracking()
                    on transaction.InventoryItemId equals item.Id
                select new { Transaction = transaction, Item = item };

            if (inventoryItemId.HasValue)
            {
                query = query.Where(x =>
                    x.Item.Id == inventoryItemId.Value);
            }

            if (fromUtc.HasValue)
            {
                query = query.Where(x =>
                    x.Transaction.CreatedAtUtc >= fromUtc.Value);
            }

            if (toUtc.HasValue)
            {
                query = query.Where(x =>
                    x.Transaction.CreatedAtUtc <= toUtc.Value);
            }

            var results = await query
                .OrderByDescending(x => x.Transaction.CreatedAtUtc)
                .Select(x => new StockMovementReport(
                    x.Transaction.Id,
                    x.Item.Id,
                    x.Item.ItemCode,
                    x.Item.Name,
                    x.Transaction.TransactionType.ToString(),
                    x.Transaction.Quantity,
                    x.Transaction.QuantityBefore,
                    x.Transaction.QuantityAfter,
                    x.Transaction.ReferenceNumber,
                    x.Transaction.Remarks,
                    x.Transaction.StorageLocationId,
                    x.Transaction.CreatedAtUtc))
                .ToListAsync(cancellationToken);

            return results;
        }

    public async Task<IReadOnlyCollection<RequisitionReport>>
    GetRequisitionReportAsync(
        RequisitionReportRequest request,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        if (request.FromUtc.HasValue &&
            request.ToUtc.HasValue &&
            request.FromUtc.Value > request.ToUtc.Value)
        {
            throw new ArgumentException(
                "The start date must not be later than the end date.",
                nameof(request));
        }

        RequisitionStatus? status = null;

        if (!string.IsNullOrWhiteSpace(request.Status))
        {
            if (!Enum.TryParse<RequisitionStatus>(
                    request.Status,
                    true,
                    out var parsedStatus) ||
                !Enum.IsDefined(parsedStatus))
            {
                throw new ArgumentException(
                    "The specified requisition status is invalid.",
                    nameof(request));
            }

            status = parsedStatus;
        }

        var query =
            from requisition in _requisitionsContext.Requisitions.AsNoTracking()
            join item in _requisitionsContext.RequisitionItems.AsNoTracking()
                on requisition.Id equals item.RequisitionId
            select new
            {
                Requisition = requisition,
                Item = item
            };

        if (request.RequestedByEmployeeId.HasValue)
        {
            query = query.Where(x =>
                x.Requisition.RequestedByEmployeeId ==
                request.RequestedByEmployeeId.Value);
        }

        if (request.DepartmentId.HasValue)
        {
            query = query.Where(x =>
                x.Requisition.DepartmentId ==
                request.DepartmentId.Value);
        }

        if (request.FiscalYearId.HasValue)
        {
            query = query.Where(x =>
                x.Requisition.FiscalYearId ==
                request.FiscalYearId.Value);
        }

        if (status.HasValue)
        {
            query = query.Where(x =>
                x.Requisition.Status == status.Value);
        }

        if (request.FromUtc.HasValue)
        {
            query = query.Where(x =>
                x.Requisition.RequestedAtUtc >= request.FromUtc.Value);
        }

        if (request.ToUtc.HasValue)
        {
            query = query.Where(x =>
                x.Requisition.RequestedAtUtc <= request.ToUtc.Value);
        }

        var records = await query
            .OrderByDescending(x => x.Requisition.RequestedAtUtc)
            .ThenBy(x => x.Requisition.RequisitionNumber)
            .ThenBy(x => x.Item.InventoryItemId)
            .Select(x => new
            {
                RequisitionId = x.Requisition.Id,
                x.Requisition.RequisitionNumber,
                x.Requisition.RequestedByEmployeeId,
                x.Requisition.DepartmentId,
                x.Requisition.FiscalYearId,
                x.Requisition.RequestedAtUtc,
                x.Requisition.Purpose,
                x.Requisition.Status,
                RequisitionItemId = x.Item.Id,
                x.Item.InventoryItemId,
                x.Item.QuantityRequested,
                ItemNotes = x.Item.Notes
            })
            .ToListAsync(cancellationToken);

        return records.Select(x => new RequisitionReport(
            x.RequisitionId,
            x.RequisitionNumber,
            x.RequestedByEmployeeId,
            x.DepartmentId,
            x.FiscalYearId,
            x.RequestedAtUtc,
            x.Purpose,
            x.Status.ToString(),
            x.RequisitionItemId,
            x.InventoryItemId,
            x.QuantityRequested,
            x.ItemNotes
        )).ToList();
    }

    public async Task<IReadOnlyCollection<DeliveryReport>> GetDeliveryReportAsync(
    DeliveryReportRequest request,
    CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        if (request.FromUtc.HasValue &&
            request.ToUtc.HasValue &&
            request.FromUtc > request.ToUtc)
        {
            throw new ArgumentException(
                "FromUtc must be earlier than or equal to ToUtc.");
        }

        var query =
            from delivery in _deliveryContext.Deliveries.AsNoTracking()
            join item in _deliveryContext.DeliveryItems.AsNoTracking()
                on delivery.Id equals item.DeliveryId
            select new { delivery, item };

        if (request.RequisitionId.HasValue)
        {
            query = query.Where(x =>
                x.delivery.RequisitionId == request.RequisitionId.Value);
        }

        if (request.PreparedByEmployeeId.HasValue)
        {
            query = query.Where(x =>
                x.delivery.PreparedByEmployeeId ==
                request.PreparedByEmployeeId.Value);
        }

        if (request.DispatchedByEmployeeId.HasValue)
        {
            query = query.Where(x =>
                x.delivery.DispatchedByEmployeeId ==
                request.DispatchedByEmployeeId.Value);
        }

        if (request.FromUtc.HasValue)
        {
            query = query.Where(x =>
                x.delivery.CreatedAtUtc >= request.FromUtc.Value);
        }

        if (request.ToUtc.HasValue)
        {
            query = query.Where(x =>
                x.delivery.CreatedAtUtc <= request.ToUtc.Value);
        }

        if (!string.IsNullOrWhiteSpace(request.Status))
        {
            if (!Enum.TryParse<DeliveryStatus>(
                    request.Status, true, out var status) ||
                !Enum.IsDefined(status))
            {
                throw new ArgumentException(
                    $"Invalid delivery status: {request.Status}");
            }

            query = query.Where(x => x.delivery.Status == status);
        }

        return await query
            .OrderByDescending(x => x.delivery.CreatedAtUtc)
            .Select(x => new DeliveryReport(
                x.delivery.Id,
                x.delivery.DeliveryNumber,
                x.delivery.RequisitionId,
                x.delivery.Status.ToString(),
                x.delivery.PreparedByEmployeeId,
                x.delivery.DispatchedByEmployeeId,
                x.delivery.CreatedAtUtc,
                x.delivery.ReadyForDispatchAtUtc,
                x.delivery.DispatchedAtUtc,
                x.delivery.Remarks,
                x.item.Id,
                x.item.InventoryItemId,
                x.item.StorageLocationId,
                x.item.QuantityPrepared))
            .ToListAsync(cancellationToken);
    }

    public async Task<InventoryOverviewSummary>
    GetInventoryOverviewSummaryAsync(
        CancellationToken cancellationToken = default)
    {
        var totalInventoryItems = await _context.InventoryItems
            .CountAsync(cancellationToken);

        var totalStockBalances = await _context.InventoryStockBalances
            .CountAsync(cancellationToken);

        var stockByItem =
            from item in _context.InventoryItems
            join balance in _context.InventoryStockBalances
                on item.Id equals balance.InventoryItemId into balances
            from balance in balances.DefaultIfEmpty()
            select new
            {
                item.Id,
                item.ReorderLevel,
                QuantityOnHand = balance == null
                    ? 0m
                    : balance.QuantityOnHand
            };

        var lowStockItemCount = await stockByItem
            .CountAsync(
                x => x.QuantityOnHand > 0 &&
                     x.QuantityOnHand <= x.ReorderLevel,
                cancellationToken);

        var outOfStockItemCount = await stockByItem
            .CountAsync(
                x => x.QuantityOnHand <= 0,
                cancellationToken);

        return new InventoryOverviewSummary(
            totalInventoryItems,
            totalStockBalances,
            lowStockItemCount,
            outOfStockItemCount);
    }

    public async Task<IReadOnlyCollection<RequisitionStatusSummary>>
        GetRequisitionStatusSummaryAsync(
            CancellationToken cancellationToken = default)
    {
        var groupedCounts = await _requisitionsContext.Requisitions
            .AsNoTracking()
            .GroupBy(requisition => requisition.Status)
            .Select(group => new
            {
                Status = group.Key,
                Count = group.Count()
            })
            .ToListAsync(cancellationToken);

        return groupedCounts
            .Select(group => new RequisitionStatusSummary(
                group.Status.ToString(),
                group.Count))
            .OrderBy(summary => summary.Status)
            .ToList();
    }
}