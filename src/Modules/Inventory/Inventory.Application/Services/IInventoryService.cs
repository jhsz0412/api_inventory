
using Inventory.Application.DTOs;
using Inventory.Domain.Entities;

namespace Inventory.Application.Services;

public interface IInventoryService
{
    Task<InventoryCategory> CreateCategoryAsync(
        CreateInventoryCategoryRequest request,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyCollection<InventoryCategory>> GetCategoriesAsync(
        CancellationToken cancellationToken = default);

    Task<UnitOfMeasure> CreateUnitOfMeasureAsync(
        CreateUnitOfMeasureRequest request,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyCollection<UnitOfMeasure>> GetUnitsOfMeasureAsync(
        CancellationToken cancellationToken = default);

    Task<InventoryItem> CreateItemAsync(
        CreateInventoryItemRequest request,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyCollection<InventoryItem>> GetItemsAsync(
        CancellationToken cancellationToken = default);

    Task<InventoryItem?> GetItemByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default);

    Task<InventoryStockBalance?> GetStockBalanceAsync(
    Guid inventoryItemId,
    CancellationToken cancellationToken = default);

    Task<InventoryStockBalance> ReceiveStockAsync(
        Guid inventoryItemId,
        decimal quantity,
        CancellationToken cancellationToken = default);

    Task<InventoryStockBalance> IssueStockAsync(
        Guid inventoryItemId,
        decimal quantity,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyCollection<InventoryStockTransaction>>
    GetStockTransactionsAsync(
        Guid inventoryItemId,
        CancellationToken cancellationToken = default);

    Task IssueStockBatchAsync(
    IReadOnlyCollection<IssueInventoryStockRequest> requests,
    string referenceNumber,
    CancellationToken cancellationToken = default);

    Task<InventoryStockBalance> AdjustStockAsync(
        Guid inventoryItemId,
        AdjustInventoryStockRequest request,
        CancellationToken cancellationToken = default);

    // Warehouse management methods
    Task<Warehouse> CreateWarehouseAsync(
    CreateWarehouseRequest request,
    CancellationToken cancellationToken = default);

    Task<IReadOnlyCollection<Warehouse>> GetWarehousesAsync(
        CancellationToken cancellationToken = default);

    Task<Warehouse?> GetWarehouseByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default);

    Task<Warehouse?> UpdateWarehouseAsync(
        Guid id,
        UpdateWarehouseRequest request,
        CancellationToken cancellationToken = default);

    Task<Warehouse?> SetWarehouseActiveStatusAsync(
        Guid id,
        bool isActive,
        CancellationToken cancellationToken = default);

    //storage location management methods
    Task<StorageLocation> CreateStorageLocationAsync(
        CreateStorageLocationRequest request,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyCollection<StorageLocation>>
        GetStorageLocationsByWarehouseIdAsync(
            Guid warehouseId,
            CancellationToken cancellationToken = default);

    Task<StorageLocation?> GetStorageLocationByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default);

    Task<StorageLocation?> UpdateStorageLocationAsync(
        Guid id,
        UpdateStorageLocationRequest request,
        CancellationToken cancellationToken = default);

    Task<StorageLocation?> SetStorageLocationActiveStatusAsync(
        Guid id,
        bool isActive,
        CancellationToken cancellationToken = default);

    // Location stock balance methods

    Task<InventoryLocationStockBalance?>
        GetLocationStockBalanceAsync(
            Guid inventoryItemId,
            Guid storageLocationId,
            CancellationToken cancellationToken = default);

    Task<IReadOnlyCollection<InventoryLocationStockBalance>>
        GetLocationStockBalancesByItemIdAsync(
            Guid inventoryItemId,
            CancellationToken cancellationToken = default);

    Task<InventoryLocationStockBalance> ReceiveStockAtLocationAsync(
        ReceiveStockAtLocationRequest request,
        CancellationToken cancellationToken = default);

    Task<InventoryLocationStockBalance> IssueStockAtLocationAsync(
        IssueStockAtLocationRequest request,
        CancellationToken cancellationToken = default);


    Task TransferStockBetweenLocationsAsync(
        TransferStockRequest request,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyCollection<InventoryStockTransaction>>
        GetLocationStockTransactionsAsync(
            Guid inventoryItemId,
            Guid storageLocationId,
            CancellationToken cancellationToken = default);

    Task<InventoryLocationStockSummaryResponse>
        GetInventoryLocationStockSummaryAsync(
            Guid inventoryItemId,
            CancellationToken cancellationToken = default);

    Task<InventoryLocationStockBalance> AdjustStockAtLocationAsync(
        AdjustStockAtLocationRequest request,
        CancellationToken cancellationToken = default);

    Task IssueStockAtLocationBatchAsync(
        IssueStockAtLocationBatchRequest request,
        CancellationToken cancellationToken = default);

}
