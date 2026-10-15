
using Inventory.Application.Interfaces;
using Inventory.Domain.Entities;
using Microsoft.EntityFrameworkCore;


namespace Inventory.Infrastructure.Persistence.Repositories;

public sealed class InventoryRepository : IInventoryRepository
{
    private readonly InventoryDbContext _context;

    public InventoryRepository(InventoryDbContext context)
    {
        _context = context;

    }

    public async Task<InventoryCategory?> GetCategoryByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        return await _context.InventoryCategories
            .FirstOrDefaultAsync(
                x => x.Id == id,
                cancellationToken);
    }

    public async Task<IReadOnlyCollection<InventoryCategory>>
        GetCategoriesAsync(
            CancellationToken cancellationToken = default)
    {
        return await _context.InventoryCategories
            .AsNoTracking()
            .OrderBy(x => x.Name)
            .ToListAsync(cancellationToken);
    }

    public async Task<UnitOfMeasure?> GetUnitOfMeasureByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        return await _context.UnitsOfMeasure
            .FirstOrDefaultAsync(
                x => x.Id == id,
                cancellationToken);
    }

    public async Task<IReadOnlyCollection<UnitOfMeasure>>
        GetUnitsOfMeasureAsync(
            CancellationToken cancellationToken = default)
    {
        return await _context.UnitsOfMeasure
            .AsNoTracking()
            .OrderBy(x => x.Name)
            .ToListAsync(cancellationToken);
    }

    public async Task<InventoryItem?> GetItemByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        return await _context.InventoryItems
            .FirstOrDefaultAsync(
                x => x.Id == id,
                cancellationToken);
    }

    public async Task<IReadOnlyCollection<InventoryItem>>
        GetItemsAsync(
            CancellationToken cancellationToken = default)
    {
        return await _context.InventoryItems
            .AsNoTracking()
            .OrderBy(x => x.Name)
            .ToListAsync(cancellationToken);
    }

    public Task<bool> CategoryCodeExistsAsync(
        string code,
        CancellationToken cancellationToken = default)
    {
        return _context.InventoryCategories
            .AnyAsync(
                x => x.Code == code.Trim().ToUpperInvariant(),
                cancellationToken);
    }

    public Task<bool> UnitCodeExistsAsync(
        string code,
        CancellationToken cancellationToken = default)
    {
        return _context.UnitsOfMeasure
            .AnyAsync(
                x => x.Code == code.Trim().ToUpperInvariant(),
                cancellationToken);
    }

    public Task<bool> ItemCodeExistsAsync(
        string code,
        CancellationToken cancellationToken = default)
    {
        return _context.InventoryItems
            .AnyAsync(
                x => x.ItemCode == code.Trim().ToUpperInvariant(),
                cancellationToken);
    }

    public async Task AddCategoryAsync(
        InventoryCategory category,
        CancellationToken cancellationToken = default)
    {
        await _context.InventoryCategories.AddAsync(
            category,
            cancellationToken);
    }

    public async Task AddUnitOfMeasureAsync(
        UnitOfMeasure unit,
        CancellationToken cancellationToken = default)
    {
        await _context.UnitsOfMeasure.AddAsync(
            unit,
            cancellationToken);
    }

    public async Task AddItemAsync(
        InventoryItem item,
        CancellationToken cancellationToken = default)
    {
        await _context.InventoryItems.AddAsync(
            item,
            cancellationToken);
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
                    "Inventory data was changed by another request. " +
                    "Please refresh the data and try again.",
                    exception);
            }
            catch (DbUpdateException exception)
                when (exception.InnerException
                          is Microsoft.Data.SqlClient.SqlException sqlException
                      && (sqlException.Number == 2601
                          || sqlException.Number == 2627))
            {
                throw new InvalidOperationException(
                    "A duplicate value violates an inventory database constraint. " +
                    "Please verify the submitted data and try again.",
                    exception);
            }
        }



    public async Task<InventoryStockBalance?> GetStockBalanceByItemIdAsync(
    Guid inventoryItemId,
    CancellationToken cancellationToken = default)
    {
        return await _context.InventoryStockBalances
            .SingleOrDefaultAsync(
                x => x.InventoryItemId == inventoryItemId,
                cancellationToken);
    }

    public async Task AddStockBalanceAsync(
        InventoryStockBalance stockBalance,
        CancellationToken cancellationToken = default)
    {
        await _context.InventoryStockBalances.AddAsync(
            stockBalance,
            cancellationToken);
    }

    public async Task<IReadOnlyCollection<InventoryStockTransaction>>
    GetStockTransactionsByItemIdAsync(
        Guid inventoryItemId,
        CancellationToken cancellationToken = default)
    {
        return await _context.InventoryStockTransactions
            .AsNoTracking()
            .Where(x => x.InventoryItemId == inventoryItemId)
            .OrderByDescending(x => x.CreatedAtUtc)
            .ToListAsync(cancellationToken);
    }

    public async Task AddStockTransactionAsync(
        InventoryStockTransaction transaction,
        CancellationToken cancellationToken = default)
    {
        await _context.InventoryStockTransactions.AddAsync(
            transaction,
            cancellationToken);
    }

    // Warehouse Methods

    public async Task<Warehouse?> GetWarehouseByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default)
        {
            return await _context.Warehouses
                .FirstOrDefaultAsync(
                    warehouse => warehouse.Id == id,
                    cancellationToken);
        }

    public async Task<IReadOnlyCollection<Warehouse>> GetWarehousesAsync(
        CancellationToken cancellationToken = default)
    {
        return await _context.Warehouses
            .OrderBy(warehouse => warehouse.Code)
            .ToListAsync(cancellationToken);
    }

    public async Task<bool> WarehouseCodeExistsAsync(
        string code,
        CancellationToken cancellationToken = default)
    {
        return await _context.Warehouses
            .AnyAsync(
                warehouse => warehouse.Code == code,
                cancellationToken);
    }

    public async Task AddWarehouseAsync(
        Warehouse warehouse,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(warehouse);

        await _context.Warehouses.AddAsync(
            warehouse,
            cancellationToken);
    }

    // Storage location methods

    public async Task<StorageLocation?> GetStorageLocationByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        return await _context.StorageLocations
            .FirstOrDefaultAsync(
                location => location.Id == id,
                cancellationToken);
    }

    public async Task<IReadOnlyCollection<StorageLocation>>
        GetStorageLocationsByWarehouseIdAsync(
            Guid warehouseId,
            CancellationToken cancellationToken = default)
    {
        return await _context.StorageLocations
            .Where(location => location.WarehouseId == warehouseId)
            .OrderBy(location => location.Code)
            .ToListAsync(cancellationToken);
    }

    public async Task<bool> StorageLocationCodeExistsAsync(
        Guid warehouseId,
        string code,
        CancellationToken cancellationToken = default)
    {
        return await _context.StorageLocations
            .AnyAsync(
                location =>
                    location.WarehouseId == warehouseId &&
                    location.Code == code,
                cancellationToken);
    }

    public async Task AddStorageLocationAsync(
        StorageLocation location,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(location);

        await _context.StorageLocations.AddAsync(
            location,
            cancellationToken);
    }

    // Location stock balance methods

    public async Task<InventoryLocationStockBalance?>
        GetLocationStockBalanceAsync(
            Guid inventoryItemId,
            Guid storageLocationId,
            CancellationToken cancellationToken = default)
    {
        return await _context.InventoryLocationStockBalances
            .SingleOrDefaultAsync(
                x => x.InventoryItemId == inventoryItemId
                    && x.StorageLocationId == storageLocationId,
                cancellationToken);
    }

    public async Task<IReadOnlyCollection<InventoryLocationStockBalance>>
        GetLocationStockBalancesByItemIdAsync(
            Guid inventoryItemId,
            CancellationToken cancellationToken = default)
    {
        return await _context.InventoryLocationStockBalances
            .AsNoTracking()
            .Where(x => x.InventoryItemId == inventoryItemId)
            .OrderBy(x => x.StorageLocationId)
            .ToListAsync(cancellationToken);
    }

    public async Task AddLocationStockBalanceAsync(
        InventoryLocationStockBalance balance,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(balance);

        await _context.InventoryLocationStockBalances
            .AddAsync(balance, cancellationToken);
    }


    public async Task<IReadOnlyCollection<InventoryStockTransaction>>
        GetLocationStockTransactionsAsync(
            Guid inventoryItemId,
            Guid storageLocationId,
            CancellationToken cancellationToken = default)
    {
        return await _context.InventoryStockTransactions
            .AsNoTracking()
            .Where(x =>
                x.InventoryItemId == inventoryItemId &&
                x.StorageLocationId == storageLocationId)
            .OrderByDescending(x => x.CreatedAtUtc)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyCollection<InventoryStockTransaction>>
    GetTransactionsByReferenceAsync(
        string referenceNumber,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(referenceNumber);

        var normalizedReference = referenceNumber.Trim();

        return await _context.InventoryStockTransactions
            .AsNoTracking()
            .Where(x => x.ReferenceNumber == normalizedReference)
            .OrderBy(x => x.CreatedAtUtc)
            .ToListAsync(cancellationToken);
    }

}
