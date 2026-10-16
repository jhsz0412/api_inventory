
using Inventory.Application.DTOs;
using Inventory.Domain.Entities;

namespace Inventory.Application.Interfaces;

public interface IInventoryRepository
{
    Task<InventoryCategory?> GetCategoryByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyCollection<InventoryCategory>> GetCategoriesAsync(
        CancellationToken cancellationToken = default);

    Task<UnitOfMeasure?> GetUnitOfMeasureByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyCollection<UnitOfMeasure>> GetUnitsOfMeasureAsync(
        CancellationToken cancellationToken = default);

    Task<InventoryItem?> GetItemByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyCollection<InventoryItem>> GetItemsAsync(
        CancellationToken cancellationToken = default);

    Task<bool> CategoryCodeExistsAsync(
        string code,
        CancellationToken cancellationToken = default);

    Task<bool> UnitCodeExistsAsync(
        string code,
        CancellationToken cancellationToken = default);

    Task<bool> ItemCodeExistsAsync(
        string code,
        CancellationToken cancellationToken = default);

    Task AddCategoryAsync(
        InventoryCategory category,
        CancellationToken cancellationToken = default);

    Task AddUnitOfMeasureAsync(
        UnitOfMeasure unit,
        CancellationToken cancellationToken = default);

    Task AddItemAsync(
        InventoryItem item,
        CancellationToken cancellationToken = default);

    Task SaveChangesAsync(
        CancellationToken cancellationToken = default);

    Task<InventoryStockBalance?> GetStockBalanceByItemIdAsync(
    Guid inventoryItemId,
    CancellationToken cancellationToken = default);

    Task AddStockBalanceAsync(
        InventoryStockBalance stockBalance,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyCollection<InventoryStockTransaction>>
    GetStockTransactionsByItemIdAsync(
        Guid inventoryItemId,
        CancellationToken cancellationToken = default);


    Task<IReadOnlyCollection<InventoryStockTransaction>>
        GetLocationStockTransactionsAsync(
            Guid inventoryItemId,
            Guid storageLocationId,
            CancellationToken cancellationToken = default);

    Task<IReadOnlyCollection<InventoryStockTransaction>>
    GetTransactionsByReferenceAsync(
        string referenceNumber,
        CancellationToken cancellationToken = default);


    Task AddStockTransactionAsync(
        InventoryStockTransaction transaction,
        CancellationToken cancellationToken = default);

    Task<Warehouse?> GetWarehouseByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyCollection<Warehouse>> GetWarehousesAsync(
        CancellationToken cancellationToken = default);

    Task<bool> WarehouseCodeExistsAsync(
        string code,
        CancellationToken cancellationToken = default);

    Task AddWarehouseAsync(
    Warehouse warehouse,
    CancellationToken cancellationToken = default);

    //storage location methods
    Task<StorageLocation?> GetStorageLocationByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyCollection<StorageLocation>>
        GetStorageLocationsByWarehouseIdAsync(
            Guid warehouseId,
            CancellationToken cancellationToken = default);

    Task<bool> StorageLocationCodeExistsAsync(
        Guid warehouseId,
        string code,
        CancellationToken cancellationToken = default);

    Task AddStorageLocationAsync(
        StorageLocation location,
        CancellationToken cancellationToken = default);

    //location stock balance methods
    Task<InventoryLocationStockBalance?>
        GetLocationStockBalanceAsync(
            Guid inventoryItemId,
            Guid storageLocationId,
            CancellationToken cancellationToken = default);

    Task<IReadOnlyCollection<InventoryLocationStockBalance>>
        GetLocationStockBalancesByItemIdAsync(
            Guid inventoryItemId,
            CancellationToken cancellationToken = default);

    Task AddLocationStockBalanceAsync(
        InventoryLocationStockBalance balance,
        CancellationToken cancellationToken = default);

    Task DeleteItemAsync(
    InventoryItem item,
    CancellationToken cancellationToken = default);



}
