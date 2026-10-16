
using Inventory.Application.DTOs;
using Inventory.Application.Interfaces;
using Inventory.Domain.Entities;

namespace Inventory.Application.Services;

public sealed class InventoryService : IInventoryService
{
    private readonly IInventoryRepository _repository;

    public InventoryService(IInventoryRepository repository)
    {
        _repository = repository;
    }

    public async Task<InventoryCategory> CreateCategoryAsync(
        CreateInventoryCategoryRequest request,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        var code = request.Code?.Trim().ToUpperInvariant()
            ?? string.Empty;

        if (await _repository.CategoryCodeExistsAsync(
                code, cancellationToken))
        {
            throw new InvalidOperationException(
                $"Category code '{code}' already exists.");
        }

        var category = new InventoryCategory(
            code,
            request.Name,
            request.Description);

        await _repository.AddCategoryAsync(
            category, cancellationToken);

        await _repository.SaveChangesAsync(cancellationToken);

        return category;
    }

    public Task<IReadOnlyCollection<InventoryCategory>>
        GetCategoriesAsync(
            CancellationToken cancellationToken = default)
    {
        return _repository.GetCategoriesAsync(cancellationToken);
    }

    public async Task<UnitOfMeasure> CreateUnitOfMeasureAsync(
        CreateUnitOfMeasureRequest request,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        var code = request.Code?.Trim().ToUpperInvariant()
            ?? string.Empty;

        if (await _repository.UnitCodeExistsAsync(
                code, cancellationToken))
        {
            throw new InvalidOperationException(
                $"Unit of measure code '{code}' already exists.");
        }

        var unit = new UnitOfMeasure(
            code,
            request.Name);

        await _repository.AddUnitOfMeasureAsync(
            unit, cancellationToken);

        await _repository.SaveChangesAsync(cancellationToken);

        return unit;
    }

    public Task<IReadOnlyCollection<UnitOfMeasure>>
        GetUnitsOfMeasureAsync(
            CancellationToken cancellationToken = default)
    {
        return _repository.GetUnitsOfMeasureAsync(cancellationToken);
    }

    public async Task<InventoryItem> CreateItemAsync(
        CreateInventoryItemRequest request,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        var code = request.ItemCode?.Trim().ToUpperInvariant()
            ?? string.Empty;

        if (await _repository.ItemCodeExistsAsync(
                code, cancellationToken))
        {
            throw new InvalidOperationException(
                $"Inventory item code '{code}' already exists.");
        }

        var category = await _repository.GetCategoryByIdAsync(
            request.CategoryId, cancellationToken);

        if (category is null)
        {
            throw new KeyNotFoundException(
                $"Category '{request.CategoryId}' was not found.");
        }

        if (!category.IsActive)
        {
            throw new InvalidOperationException(
                "Cannot create an item in an inactive category.");
        }

        var unit = await _repository.GetUnitOfMeasureByIdAsync(
            request.UnitOfMeasureId, cancellationToken);

        if (unit is null)
        {
            throw new KeyNotFoundException(
                $"Unit of measure '{request.UnitOfMeasureId}' was not found.");
        }

        if (!unit.IsActive)
        {
            throw new InvalidOperationException(
                "Cannot create an item with an inactive unit of measure.");
        }

        var item = new InventoryItem(
            code,
            request.Name,
            request.CategoryId,
            request.UnitOfMeasureId,
            request.Description,
            request.ReorderLevel);

        await _repository.AddItemAsync(item, cancellationToken);

        await _repository.SaveChangesAsync(cancellationToken);

        return item;
    }

    public Task<IReadOnlyCollection<InventoryItem>> GetItemsAsync(
        CancellationToken cancellationToken = default)
    {
        return _repository.GetItemsAsync(cancellationToken);
    }

    public Task<InventoryItem?> GetItemByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        return _repository.GetItemByIdAsync(id, cancellationToken);
    }

    public async Task<InventoryStockBalance?> GetStockBalanceAsync(
    Guid inventoryItemId,
    CancellationToken cancellationToken = default)
    {
        return await _repository.GetStockBalanceByItemIdAsync(
            inventoryItemId,
            cancellationToken);
    }

    public async Task<InventoryStockBalance> ReceiveStockAsync(
        Guid inventoryItemId,
        decimal quantity,
        CancellationToken cancellationToken = default)
    {
        var item = await _repository.GetItemByIdAsync(
            inventoryItemId,
            cancellationToken);

        if (item is null)
        {
            throw new KeyNotFoundException(
                "Inventory item was not found.");
        }

        var balance = await _repository.GetStockBalanceByItemIdAsync(
            inventoryItemId,
            cancellationToken);

        if (balance is null)
        {
            balance = new InventoryStockBalance(inventoryItemId);

            await _repository.AddStockBalanceAsync(
                balance,
                cancellationToken);
        }

        var quantityBefore = balance.QuantityOnHand;

        balance.IncreaseStock(quantity);

        var transaction = new InventoryStockTransaction(
            inventoryItemId,
            InventoryStockTransactionType.StockIn,
            quantity,
            quantityBefore,
            balance.QuantityOnHand);

        await _repository.AddStockTransactionAsync(
            transaction,
            cancellationToken);

        await _repository.SaveChangesAsync(cancellationToken);

        return balance;
    }

    public async Task<InventoryStockBalance> IssueStockAsync(
        Guid inventoryItemId,
        decimal quantity,
        CancellationToken cancellationToken = default)
    {
        var item = await _repository.GetItemByIdAsync(
            inventoryItemId,
            cancellationToken);

        if (item is null)
        {
            throw new KeyNotFoundException(
                "Inventory item was not found.");
        }

        var balance = await _repository.GetStockBalanceByItemIdAsync(
            inventoryItemId,
            cancellationToken);

        if (balance is null)
        {
            throw new InvalidOperationException(
                "No stock balance exists for this inventory item.");
        }

        var quantityBefore = balance.QuantityOnHand;

        balance.DecreaseStock(quantity);

        var transaction = new InventoryStockTransaction(
            inventoryItemId,
            InventoryStockTransactionType.StockOut,
            quantity,
            quantityBefore,
            balance.QuantityOnHand);

        await _repository.AddStockTransactionAsync(
            transaction,
            cancellationToken);

        await _repository.SaveChangesAsync(cancellationToken);

        return balance;
    }

    public async Task<IReadOnlyCollection<InventoryStockTransaction>>
    GetStockTransactionsAsync(
        Guid inventoryItemId,
        CancellationToken cancellationToken = default)
    {
        var item = await _repository.GetItemByIdAsync(
            inventoryItemId,
            cancellationToken);

        if (item is null)
        {
            throw new KeyNotFoundException(
                "Inventory item was not found.");
        }

        return await _repository.GetStockTransactionsByItemIdAsync(
            inventoryItemId,
            cancellationToken);
    }



    public async Task IssueStockBatchAsync(
        IReadOnlyCollection<IssueInventoryStockRequest> requests,
        string referenceNumber,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(requests);

        if (requests.Count == 0)
            throw new ArgumentException(
                "At least one stock issue request is required.",
                nameof(requests));

        if (string.IsNullOrWhiteSpace(referenceNumber))
            throw new ArgumentException(
                "A reference number is required.",
                nameof(referenceNumber));

        // Combine duplicate lines for the same inventory item.
        var groupedRequests = requests
            .GroupBy(x => x.InventoryItemId)
            .ToDictionary(
                group => group.Key,
                group => group.Sum(x => x.Quantity));

        foreach (var request in groupedRequests)
        {
            if (request.Key == Guid.Empty)
                throw new ArgumentException(
                    "Inventory item ID is required.");

            if (request.Value <= 0)
                throw new ArgumentOutOfRangeException(
                    nameof(requests),
                    "Every requested quantity must be greater than zero.");
        }

        // Check whether this reference has already issued stock.
        var existingTransactions =
            new Dictionary<Guid, InventoryStockTransaction>();

        foreach (var request in groupedRequests)
        {
            var transactions =
                await _repository.GetStockTransactionsByItemIdAsync(
                    request.Key,
                    cancellationToken);

            var matchingTransactions = transactions
                .Where(transaction =>
                    transaction.ReferenceNumber == referenceNumber &&
                    transaction.TransactionType ==
                        InventoryStockTransactionType.StockOut)
                .ToArray();

            if (matchingTransactions.Length > 1)
                throw new InvalidOperationException(
                    $"Multiple stock-out transactions already exist for " +
                    $"inventory item '{request.Key}' and reference '{referenceNumber}'.");

            if (matchingTransactions.Length == 1)
            {
                existingTransactions.Add(
                    request.Key,
                    matchingTransactions[0]);
            }
        }

        // If any stock was previously issued, verify the entire batch.
        if (existingTransactions.Count > 0)
        {
            if (existingTransactions.Count != groupedRequests.Count)
                throw new InvalidOperationException(
                    $"A partial stock issue already exists for reference " +
                    $"'{referenceNumber}'. Manual reconciliation is required.");

            foreach (var request in groupedRequests)
            {
                var transaction = existingTransactions[request.Key];

                if (transaction.Quantity != request.Value)
                    throw new InvalidOperationException(
                        $"The previously issued quantity for inventory item " +
                        $"'{request.Key}' does not match the requested quantity. " +
                        "Manual reconciliation is required.");
            }

            // The complete batch was already issued with matching quantities.
            // Return without deducting stock again.
            return;
        }

        // No previous stock issue exists. Validate stock availability.
        var balances = new Dictionary<Guid, InventoryStockBalance>();

        foreach (var request in groupedRequests)
        {
            var item = await _repository.GetItemByIdAsync(
                request.Key,
                cancellationToken);

            if (item is null)
                throw new KeyNotFoundException(
                    $"Inventory item '{request.Key}' was not found.");

            var balance = await _repository.GetStockBalanceByItemIdAsync(
                request.Key,
                cancellationToken);

            if (balance is null ||
                balance.QuantityOnHand < request.Value)
            {
                throw new InvalidOperationException(
                    $"Insufficient stock for inventory item '{request.Key}'.");
            }

            balances.Add(request.Key, balance);
        }

        // Create one stock-out transaction per inventory item.
        foreach (var request in groupedRequests)
        {
            var balance = balances[request.Key];
            var quantityBefore = balance.QuantityOnHand;

            balance.DecreaseStock(request.Value);

            var transaction = new InventoryStockTransaction(
                request.Key,
                InventoryStockTransactionType.StockOut,
                request.Value,
                quantityBefore,
                balance.QuantityOnHand,
                referenceNumber);

            await _repository.AddStockTransactionAsync(
                transaction,
                cancellationToken);
        }

        // Persist the complete batch in one SaveChanges operation.
        await _repository.SaveChangesAsync(cancellationToken);
    }

public async Task<InventoryStockBalance> AdjustStockAsync(
    Guid inventoryItemId,
    AdjustInventoryStockRequest request,
    CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        if (inventoryItemId == Guid.Empty)
        {
            throw new ArgumentException(
                "Inventory item ID is required.",
                nameof(inventoryItemId));
        }

        if (request.NewQuantity < 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(request.NewQuantity),
                "The new stock quantity cannot be negative.");
        }

        if (string.IsNullOrWhiteSpace(request.Remarks))
        {
            throw new ArgumentException(
                "Remarks are required for stock adjustments.",
                nameof(request.Remarks));
        }

        var item = await _repository.GetItemByIdAsync(
            inventoryItemId,
            cancellationToken);

        if (item is null)
        {
            throw new KeyNotFoundException(
                "Inventory item was not found.");
        }

        var balance = await _repository.GetStockBalanceByItemIdAsync(
            inventoryItemId,
            cancellationToken);

        if (balance is null)
        {
            throw new InvalidOperationException(
                "No stock balance exists for this inventory item.");
        }

        var quantityBefore = balance.QuantityOnHand;

        if (quantityBefore == request.NewQuantity)
        {
            throw new InvalidOperationException(
                "The new quantity matches the current stock balance. " +
                "No adjustment is necessary.");
        }

        var adjustmentQuantity = Math.Abs(
            request.NewQuantity - quantityBefore);

        balance.AdjustStock(request.NewQuantity);

        var transaction = new InventoryStockTransaction(
            inventoryItemId,
            InventoryStockTransactionType.Adjustment,
            adjustmentQuantity,
            quantityBefore,
            balance.QuantityOnHand,
            remarks: request.Remarks.Trim());

        await _repository.AddStockTransactionAsync(
            transaction,
            cancellationToken);

        await _repository.SaveChangesAsync(cancellationToken);

        return balance;
    }

    //warehouse management
    public async Task<Warehouse> CreateWarehouseAsync(
        CreateWarehouseRequest request,
        CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(request);

            var code = request.Code?.Trim().ToUpperInvariant()
                ?? string.Empty;

            if (await _repository.WarehouseCodeExistsAsync(
                    code, cancellationToken))
            {
                throw new InvalidOperationException(
                    $"Warehouse code '{code}' already exists.");
            }

            var warehouse = new Warehouse(
                code,
                request.Name,
                request.Description);

            await _repository.AddWarehouseAsync(
                warehouse, cancellationToken);

            await _repository.SaveChangesAsync(cancellationToken);

            return warehouse;
        }

        public Task<IReadOnlyCollection<Warehouse>> GetWarehousesAsync(
            CancellationToken cancellationToken = default)
        {
            return _repository.GetWarehousesAsync(cancellationToken);
        }

        public Task<Warehouse?> GetWarehouseByIdAsync(
            Guid id,
            CancellationToken cancellationToken = default)
        {
            return _repository.GetWarehouseByIdAsync(
                id, cancellationToken);
        }

        public async Task<Warehouse?> UpdateWarehouseAsync(
            Guid id,
            UpdateWarehouseRequest request,
            CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(request);

            var warehouse = await _repository.GetWarehouseByIdAsync(
                id, cancellationToken);

            if (warehouse is null)
                return null;

            warehouse.UpdateDetails(
                request.Name,
                request.Description);

            await _repository.SaveChangesAsync(cancellationToken);

            return warehouse;
        }

        public async Task<Warehouse?> SetWarehouseActiveStatusAsync(
            Guid id,
            bool isActive,
            CancellationToken cancellationToken = default)
        {
            var warehouse = await _repository.GetWarehouseByIdAsync(
                id, cancellationToken);

            if (warehouse is null)
                return null;

            if (isActive)
                warehouse.Activate();
            else
                warehouse.Deactivate();

            await _repository.SaveChangesAsync(cancellationToken);

            return warehouse;
    }

    //storage location management
    public async Task<StorageLocation> CreateStorageLocationAsync(
        CreateStorageLocationRequest request,
        CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(request);

            var warehouse = await _repository.GetWarehouseByIdAsync(
                request.WarehouseId,
                cancellationToken);

            if (warehouse is null)
            {
                throw new KeyNotFoundException(
                    $"Warehouse '{request.WarehouseId}' was not found.");
            }

            if (!warehouse.IsActive)
            {
                throw new InvalidOperationException(
                    "Cannot create a storage location in an inactive warehouse.");
            }

            var code = request.Code?.Trim().ToUpperInvariant()
                ?? string.Empty;

            if (await _repository.StorageLocationCodeExistsAsync(
                    request.WarehouseId,
                    code,
                    cancellationToken))
            {
                throw new InvalidOperationException(
                    $"Storage location code '{code}' already exists in this warehouse.");
            }

            var location = new StorageLocation(
                request.WarehouseId,
                code,
                request.Name,
                request.Description);

            await _repository.AddStorageLocationAsync(
                location,
                cancellationToken);

            await _repository.SaveChangesAsync(cancellationToken);

            return location;
        }

    public async Task<IReadOnlyCollection<StorageLocation>>
        GetStorageLocationsByWarehouseIdAsync(
            Guid warehouseId,
            CancellationToken cancellationToken = default)
    {
        var warehouse = await _repository.GetWarehouseByIdAsync(
            warehouseId,
            cancellationToken);

        if (warehouse is null)
        {
            throw new KeyNotFoundException(
                $"Warehouse '{warehouseId}' was not found.");
        }

        return await _repository.GetStorageLocationsByWarehouseIdAsync(
            warehouseId,
            cancellationToken);
    }

    public Task<StorageLocation?> GetStorageLocationByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        return _repository.GetStorageLocationByIdAsync(
            id, cancellationToken);
    }

    public async Task<StorageLocation?> UpdateStorageLocationAsync(
        Guid id,
        UpdateStorageLocationRequest request,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        var location = await _repository.GetStorageLocationByIdAsync(
            id,
            cancellationToken);

        if (location is null)
            return null;

        location.UpdateDetails(
            request.Name,
            request.Description);

        await _repository.SaveChangesAsync(cancellationToken);

        return location;
    }

    public async Task<StorageLocation?> SetStorageLocationActiveStatusAsync(
        Guid id,
        bool isActive,
        CancellationToken cancellationToken = default)
    {
        var location = await _repository.GetStorageLocationByIdAsync(
            id,
            cancellationToken);

        if (location is null)
            return null;

        if (isActive)
            location.Activate();
        else
            location.Deactivate();

        await _repository.SaveChangesAsync(cancellationToken);

        return location;
    }

    public async Task<InventoryLocationStockBalance?>
        GetLocationStockBalanceAsync(
            Guid inventoryItemId,
            Guid storageLocationId,
            CancellationToken cancellationToken = default)
    {
        if (inventoryItemId == Guid.Empty)
        {
            throw new ArgumentException(
                "Inventory item ID is required.",
                nameof(inventoryItemId));
        }

        if (storageLocationId == Guid.Empty)
        {
            throw new ArgumentException(
                "Storage location ID is required.",
                nameof(storageLocationId));
        }

        return await _repository.GetLocationStockBalanceAsync(
            inventoryItemId,
            storageLocationId,
            cancellationToken);
    }

    public async Task<IReadOnlyCollection<InventoryLocationStockBalance>>
        GetLocationStockBalancesByItemIdAsync(
            Guid inventoryItemId,
            CancellationToken cancellationToken = default)
    {
        if (inventoryItemId == Guid.Empty)
        {
            throw new ArgumentException(
                "Inventory item ID is required.",
                nameof(inventoryItemId));
        }

        return await _repository.GetLocationStockBalancesByItemIdAsync(
            inventoryItemId,
            cancellationToken);
    }

    public async Task<InventoryLocationStockBalance> ReceiveStockAtLocationAsync(
        ReceiveStockAtLocationRequest request,
        CancellationToken cancellationToken = default)
        {
        ArgumentNullException.ThrowIfNull(request);


        if (request.InventoryItemId == Guid.Empty)
            throw new ArgumentException("Inventory item ID is required.");

        if (request.StorageLocationId == Guid.Empty)
            throw new ArgumentException("Storage location ID is required.");

        if (request.Quantity <= 0)
            throw new ArgumentException("Received quantity must be greater than zero.");

        if (string.IsNullOrWhiteSpace(request.ReferenceNumber))
            throw new ArgumentException(
                "Reference number is required for stock receiving.");

        var existingTransactions =
    await _repository.GetTransactionsByReferenceAsync(
        request.ReferenceNumber,
        cancellationToken);

        if (existingTransactions.Any(x =>
                x.InventoryItemId == request.InventoryItemId &&
                x.StorageLocationId == request.StorageLocationId &&
                x.TransactionType == InventoryStockTransactionType.StockIn))
        {
            throw new InvalidOperationException(
                $"Stock receiving reference '{request.ReferenceNumber}' has already been processed.");
        }

        var item = await _repository.GetItemByIdAsync(
            request.InventoryItemId,
            cancellationToken);

        if (item is null)
            throw new KeyNotFoundException(
                $"Inventory item '{request.InventoryItemId}' was not found.");

        var location = await _repository.GetStorageLocationByIdAsync(
            request.StorageLocationId,
            cancellationToken);

        if (location is null)
            throw new KeyNotFoundException(
                $"Storage location '{request.StorageLocationId}' was not found.");

        if (!location.IsActive)
            throw new InvalidOperationException(
                "Stock cannot be received into an inactive storage location.");

        var balance = await _repository.GetLocationStockBalanceAsync(
            request.InventoryItemId,
            request.StorageLocationId,
            cancellationToken);

        if (balance is null)
        {
            balance = new InventoryLocationStockBalance(
                request.InventoryItemId,
                request.StorageLocationId);

            await _repository.AddLocationStockBalanceAsync(
                balance,
                cancellationToken);
        }

        var quantityBefore = balance.QuantityOnHand;

        balance.IncreaseStock(request.Quantity);

        var transaction = new InventoryStockTransaction(
            request.InventoryItemId,
            InventoryStockTransactionType.StockIn,
            request.Quantity,
            quantityBefore,
            balance.QuantityOnHand,
            request.ReferenceNumber,
            request.Remarks);

        transaction.AssignStorageLocation(request.StorageLocationId);

        await _repository.AddStockTransactionAsync(
            transaction,
            cancellationToken);

        await _repository.SaveChangesAsync(cancellationToken);

        return balance;
    }
    public async Task<InventoryLocationStockBalance> IssueStockAtLocationAsync(
        IssueStockAtLocationRequest request,
        CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(request);

            if (request.InventoryItemId == Guid.Empty)
                throw new ArgumentException("Inventory item ID is required.");

            if (request.StorageLocationId == Guid.Empty)
                throw new ArgumentException("Storage location ID is required.");

            if (request.Quantity <= 0)
                throw new ArgumentException("Issued quantity must be greater than zero.");

            var item = await _repository.GetItemByIdAsync(
                request.InventoryItemId,
                cancellationToken);

            if (item is null)
                throw new KeyNotFoundException(
                    $"Inventory item '{request.InventoryItemId}' was not found.");

            var location = await _repository.GetStorageLocationByIdAsync(
                request.StorageLocationId,
                cancellationToken);

            if (location is null)
                throw new KeyNotFoundException(
                    $"Storage location '{request.StorageLocationId}' was not found.");

            if (!location.IsActive)
                throw new InvalidOperationException(
                    "Stock cannot be issued from an inactive storage location.");

            var balance = await _repository.GetLocationStockBalanceAsync(
                request.InventoryItemId,
                request.StorageLocationId,
                cancellationToken);

            if (balance is null)
                throw new InvalidOperationException(
                    "No stock balance exists for this item at the selected location.");

            if (balance.QuantityOnHand < request.Quantity)
                throw new InvalidOperationException(
                    $"Insufficient stock at this location. Requested: {request.Quantity}, " +
                    $"Available: {balance.QuantityOnHand}.");

            var quantityBefore = balance.QuantityOnHand;

            balance.DecreaseStock(request.Quantity);

            var transaction = new InventoryStockTransaction(
                request.InventoryItemId,
                InventoryStockTransactionType.StockOut,
                request.Quantity,
                quantityBefore,
                balance.QuantityOnHand,
                request.ReferenceNumber,
                request.Remarks);

            transaction.AssignStorageLocation(request.StorageLocationId);

            await _repository.AddStockTransactionAsync(
                transaction,
                cancellationToken);

            await _repository.SaveChangesAsync(cancellationToken);

            return balance;
    }

    public async Task TransferStockBetweenLocationsAsync(
        TransferStockRequest request,
        CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(request);

        if (string.IsNullOrWhiteSpace(request.ReferenceNumber))
            throw new ArgumentException(
                "Reference number is required for stock transfer.");

        if (request.InventoryItemId == Guid.Empty)
                throw new ArgumentException("Inventory item ID is required.");

            if (request.SourceStorageLocationId == Guid.Empty)
                throw new ArgumentException("Source storage location ID is required.");

            if (request.DestinationStorageLocationId == Guid.Empty)
                throw new ArgumentException("Destination storage location ID is required.");

            if (request.SourceStorageLocationId == request.DestinationStorageLocationId)
                throw new ArgumentException(
                    "Source and destination storage locations must be different.");

            if (request.Quantity <= 0)
                throw new ArgumentException("Transfer quantity must be greater than zero.");

        var existingTransactions =
            await _repository.GetTransactionsByReferenceAsync(
                request.ReferenceNumber,
                cancellationToken);

            if (existingTransactions.Any(x =>
                    x.InventoryItemId == request.InventoryItemId &&
                    x.TransactionType == InventoryStockTransactionType.StockOut))
            {
                throw new InvalidOperationException(
                    $"Stock transfer reference '{request.ReferenceNumber}' has already been processed.");
            }

        var item = await _repository.GetItemByIdAsync(
                request.InventoryItemId,
                cancellationToken);

            if (item is null)
                throw new KeyNotFoundException(
                    $"Inventory item '{request.InventoryItemId}' was not found.");

            var sourceLocation = await _repository.GetStorageLocationByIdAsync(
                request.SourceStorageLocationId,
                cancellationToken);

            if (sourceLocation is null)
                throw new KeyNotFoundException(
                    $"Source storage location '{request.SourceStorageLocationId}' was not found.");

            var destinationLocation = await _repository.GetStorageLocationByIdAsync(
                request.DestinationStorageLocationId,
                cancellationToken);

            if (destinationLocation is null)
                throw new KeyNotFoundException(
                    $"Destination storage location '{request.DestinationStorageLocationId}' was not found.");

            if (!sourceLocation.IsActive || !destinationLocation.IsActive)
                throw new InvalidOperationException(
                    "Stock can only be transferred between active storage locations.");

            var sourceBalance = await _repository.GetLocationStockBalanceAsync(
                request.InventoryItemId,
                request.SourceStorageLocationId,
                cancellationToken);

            if (sourceBalance is null)
                throw new InvalidOperationException(
                    "No stock balance exists at the source storage location.");

            if (sourceBalance.QuantityOnHand < request.Quantity)
                throw new InvalidOperationException(
                    $"Insufficient stock at the source location. Requested: {request.Quantity}, " +
                    $"Available: {sourceBalance.QuantityOnHand}.");

            var destinationBalance = await _repository.GetLocationStockBalanceAsync(
                request.InventoryItemId,
                request.DestinationStorageLocationId,
                cancellationToken);

            if (destinationBalance is null)
            {
                destinationBalance = new InventoryLocationStockBalance(
                    request.InventoryItemId,
                    request.DestinationStorageLocationId);

                await _repository.AddLocationStockBalanceAsync(
                    destinationBalance,
                    cancellationToken);
            }

            var sourceQuantityBefore = sourceBalance.QuantityOnHand;
            var destinationQuantityBefore = destinationBalance.QuantityOnHand;

            sourceBalance.DecreaseStock(request.Quantity);
            destinationBalance.IncreaseStock(request.Quantity);

            var sourceTransaction = new InventoryStockTransaction(
                request.InventoryItemId,
                InventoryStockTransactionType.StockOut,
                request.Quantity,
                sourceQuantityBefore,
                sourceBalance.QuantityOnHand,
                request.ReferenceNumber,
                request.Remarks ?? "Stock transfer out");

            sourceTransaction.AssignStorageLocation(
                request.SourceStorageLocationId);

            var destinationTransaction = new InventoryStockTransaction(
                request.InventoryItemId,
                InventoryStockTransactionType.StockIn,
                request.Quantity,
                destinationQuantityBefore,
                destinationBalance.QuantityOnHand,
                request.ReferenceNumber,
                request.Remarks ?? "Stock transfer in");

            destinationTransaction.AssignStorageLocation(
                request.DestinationStorageLocationId);

            await _repository.AddStockTransactionAsync(
                sourceTransaction,
                cancellationToken);

            await _repository.AddStockTransactionAsync(
                destinationTransaction,
                cancellationToken);

            await _repository.SaveChangesAsync(cancellationToken);
    }

    public async Task<IReadOnlyCollection<InventoryStockTransaction>>
        GetLocationStockTransactionsAsync(
            Guid inventoryItemId,
            Guid storageLocationId,
            CancellationToken cancellationToken = default)
        {
            if (inventoryItemId == Guid.Empty)
                throw new ArgumentException("Inventory item ID is required.");

            if (storageLocationId == Guid.Empty)
                throw new ArgumentException("Storage location ID is required.");

            var item = await _repository.GetItemByIdAsync(
                inventoryItemId,
                cancellationToken);

            if (item is null)
                throw new KeyNotFoundException(
                    $"Inventory item '{inventoryItemId}' was not found.");

            var location = await _repository.GetStorageLocationByIdAsync(
                storageLocationId,
                cancellationToken);

            if (location is null)
                throw new KeyNotFoundException(
                    $"Storage location '{storageLocationId}' was not found.");

        return await _repository.GetLocationStockTransactionsAsync(
            inventoryItemId,
            storageLocationId,
            cancellationToken);
    }

    public async Task<InventoryLocationStockSummaryResponse>
        GetInventoryLocationStockSummaryAsync(
            Guid inventoryItemId,
            CancellationToken cancellationToken = default)
        {
            if (inventoryItemId == Guid.Empty)
                throw new ArgumentException("Inventory item ID is required.");

            var item = await _repository.GetItemByIdAsync(
                inventoryItemId,
                cancellationToken);

            if (item is null)
                throw new KeyNotFoundException(
                    $"Inventory item '{inventoryItemId}' was not found.");

            var balances = await _repository.GetLocationStockBalancesByItemIdAsync(
                inventoryItemId,
                cancellationToken);

            var locations = balances
                .Select(balance =>
                    new InventoryLocationStockSummaryLocationResponse(
                        balance.StorageLocationId,
                        balance.QuantityOnHand))
                .ToArray();

            return new InventoryLocationStockSummaryResponse(
                inventoryItemId,
                balances.Sum(balance => balance.QuantityOnHand),
                locations.Length,
                locations);
    }

    public async Task<InventoryLocationStockBalance>
        AdjustStockAtLocationAsync(
            AdjustStockAtLocationRequest request,
            CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(request);

            if (request.InventoryItemId == Guid.Empty)
                throw new ArgumentException("Inventory item ID is required.");

            if (request.StorageLocationId == Guid.Empty)
                throw new ArgumentException("Storage location ID is required.");

            if (request.NewQuantity < 0)
                throw new ArgumentOutOfRangeException(
                    nameof(request.NewQuantity),
                    "New quantity cannot be negative.");

            if (string.IsNullOrWhiteSpace(request.Remarks))
                throw new ArgumentException(
                    "Adjustment remarks are required.",
                    nameof(request.Remarks));

            var item = await _repository.GetItemByIdAsync(
                request.InventoryItemId,
                cancellationToken);

            if (item is null)
                throw new KeyNotFoundException(
                    $"Inventory item '{request.InventoryItemId}' was not found.");

            var location = await _repository.GetStorageLocationByIdAsync(
                request.StorageLocationId,
                cancellationToken);

            if (location is null)
                throw new KeyNotFoundException(
                    $"Storage location '{request.StorageLocationId}' was not found.");

            if (!location.IsActive)
                throw new InvalidOperationException(
                    "Cannot adjust stock at an inactive storage location.");

            var balance = await _repository.GetLocationStockBalanceAsync(
                request.InventoryItemId,
                request.StorageLocationId,
                cancellationToken);

            if (balance is null)
                throw new InvalidOperationException(
                    "No stock balance exists for this item at the specified location. Receive stock first.");

            var quantityBefore = balance.QuantityOnHand;

            if (quantityBefore == request.NewQuantity)
                throw new InvalidOperationException(
                    "The new quantity is the same as the current quantity.");

            balance.AdjustStock(request.NewQuantity);

            var transaction = new InventoryStockTransaction(
                request.InventoryItemId,
                InventoryStockTransactionType.Adjustment,
                Math.Abs(request.NewQuantity - quantityBefore),
                quantityBefore,
                request.NewQuantity,
                null,
                request.Remarks.Trim());

            transaction.AssignStorageLocation(request.StorageLocationId);

            await _repository.AddStockTransactionAsync(
                transaction,
                cancellationToken);

            await _repository.SaveChangesAsync(cancellationToken);

            return balance;
        }

    public async Task IssueStockAtLocationBatchAsync(
    IssueStockAtLocationBatchRequest request,
    CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        if (string.IsNullOrWhiteSpace(request.ReferenceNumber))
            throw new ArgumentException("A reference number is required.");

        if (request.Items is null || request.Items.Count == 0)
            throw new ArgumentException(
                "At least one stock issue item is required.");

        var referenceNumber = request.ReferenceNumber.Trim();

        // Validate every line before combining quantities.
        foreach (var item in request.Items)
        {
            if (item.InventoryItemId == Guid.Empty)
                throw new ArgumentException("Inventory item ID is required.");

            if (item.StorageLocationId == Guid.Empty)
                throw new ArgumentException("Storage location ID is required.");

            if (item.Quantity <= 0)
                throw new ArgumentException(
                    "Every issued quantity must be greater than zero.");
        }

        // Combine duplicate lines for the same item and storage location.
        var groupedRequests = request.Items
            .GroupBy(x => (x.InventoryItemId, x.StorageLocationId))
            .Select(group => new
            {
                group.Key.InventoryItemId,
                group.Key.StorageLocationId,
                Quantity = group.Sum(x => x.Quantity)
            })
            .ToArray();

        // Check for an existing stock issue using the same reference.
        var existingTransactions = new List<InventoryStockTransaction>();

        foreach (var inventoryItemId in groupedRequests
                     .Select(x => x.InventoryItemId)
                     .Distinct())
        {
            var transactions =
                await _repository.GetStockTransactionsByItemIdAsync(
                    inventoryItemId,
                    cancellationToken);

            existingTransactions.AddRange(
                transactions.Where(transaction =>
                    transaction.ReferenceNumber == referenceNumber &&
                    transaction.TransactionType ==
                        InventoryStockTransactionType.StockOut));
        }

        if (existingTransactions.Count > 0)
        {
            var existingByLocation = existingTransactions
                .GroupBy(transaction =>
                    (transaction.InventoryItemId,
                     transaction.StorageLocationId))
                .ToDictionary(group => group.Key, group => group.ToArray());

            if (existingTransactions.Count != groupedRequests.Length)
                throw new InvalidOperationException(
                    $"A partial stock issue already exists for reference " +
                    $"'{referenceNumber}'. Manual reconciliation is required.");

            foreach (var item in groupedRequests)
            {
                var key = (item.InventoryItemId, item.StorageLocationId);

                if (!existingByLocation.TryGetValue(key, out var matches) ||
                    matches.Length != 1 ||
                    matches[0].Quantity != item.Quantity)
                {
                    throw new InvalidOperationException(
                        $"The existing stock issue for reference " +
                        $"'{referenceNumber}' does not match this batch.");
                }
            }

            // The matching batch was already issued. Do not deduct stock again.
            return;
        }

        // Validate all items, locations, and balances before changing any balance.
        var balances =
            new Dictionary<(Guid InventoryItemId, Guid StorageLocationId),
                InventoryLocationStockBalance>();

        foreach (var item in groupedRequests)
        {
            var inventoryItem = await _repository.GetItemByIdAsync(
                item.InventoryItemId,
                cancellationToken);

            if (inventoryItem is null)
                throw new KeyNotFoundException(
                    $"Inventory item '{item.InventoryItemId}' was not found.");

            var location = await _repository.GetStorageLocationByIdAsync(
                item.StorageLocationId,
                cancellationToken);

            if (location is null)
                throw new KeyNotFoundException(
                    $"Storage location '{item.StorageLocationId}' was not found.");

            if (!location.IsActive)
                throw new InvalidOperationException(
                    $"Storage location '{item.StorageLocationId}' is inactive.");

            var balance = await _repository.GetLocationStockBalanceAsync(
                item.InventoryItemId,
                item.StorageLocationId,
                cancellationToken);

            if (balance is null || balance.QuantityOnHand < item.Quantity)
            {
                throw new InvalidOperationException(
                    $"Insufficient stock for item '{item.InventoryItemId}' " +
                    $"at location '{item.StorageLocationId}'.");
            }

            balances.Add(
                (item.InventoryItemId, item.StorageLocationId),
                balance);
        }

        // Apply the deductions and create the corresponding stock transactions.
        foreach (var item in groupedRequests)
        {
            var balance = balances[
                (item.InventoryItemId, item.StorageLocationId)];

            var quantityBefore = balance.QuantityOnHand;

            balance.DecreaseStock(item.Quantity);

            var transaction = new InventoryStockTransaction(
                item.InventoryItemId,
                InventoryStockTransactionType.StockOut,
                item.Quantity,
                quantityBefore,
                balance.QuantityOnHand,
                referenceNumber,
                request.Remarks);

            transaction.AssignStorageLocation(item.StorageLocationId);

            await _repository.AddStockTransactionAsync(
                transaction,
                cancellationToken);
        }

        // Persist all balance changes and transactions together.
        await _repository.SaveChangesAsync(cancellationToken);
    }


    public async Task<InventoryItem?> UpdateItemAsync(
        Guid id,
        UpdateInventoryItemRequest request,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        var item = await _repository.GetItemByIdAsync(
            id,
            cancellationToken);

        if (item is null)
            return null;

        var category = await _repository.GetCategoryByIdAsync(
            request.CategoryId,
            cancellationToken);

        if (category is null)
        {
            throw new KeyNotFoundException(
                $"Category '{request.CategoryId}' was not found.");
        }

        if (!category.IsActive)
        {
            throw new InvalidOperationException(
                "Cannot assign an item to an inactive category.");
        }

        var unit = await _repository.GetUnitOfMeasureByIdAsync(
            request.UnitOfMeasureId,
            cancellationToken);

        if (unit is null)
        {
            throw new KeyNotFoundException(
                $"Unit of measure '{request.UnitOfMeasureId}' was not found.");
        }

        if (!unit.IsActive)
        {
            throw new InvalidOperationException(
                "Cannot assign an inactive unit of measure to an item.");
        }

        item.Update(
            request.Name,
            request.Description,
            request.CategoryId,
            request.UnitOfMeasureId,
            request.ReorderLevel);

        await _repository.SaveChangesAsync(cancellationToken);

        return item;
    }

    public async Task<bool> DeactivateItemAsync(
    Guid id,
    CancellationToken cancellationToken = default)
    {
        var item = await _repository.GetItemByIdAsync(
            id,
            cancellationToken);

        if (item is null)
            return false;

        if (!item.IsActive)
            throw new InvalidOperationException(
                "The inventory item is already inactive.");

        item.Deactivate();

        await _repository.SaveChangesAsync(cancellationToken);

        return true;
    }


    public async Task<bool> RestoreItemAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        var item = await _repository.GetItemByIdAsync(
            id,
            cancellationToken);

        if (item is null)
            return false;

        if (item.IsActive)
        {
            throw new InvalidOperationException(
                "The inventory item is already active.");
        }

        item.Activate();

        await _repository.SaveChangesAsync(cancellationToken);

        return true;
    }


    public async Task<bool> PermanentlyDeleteItemAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        var item = await _repository.GetItemByIdAsync(
            id,
            cancellationToken);

        if (item is null)
            return false;

        if (item.IsActive)
        {
            throw new InvalidOperationException(
                "Only inactive inventory items can be permanently deleted. " +
                "Deactivate the item first.");
        }

        var stockBalance = await _repository.GetStockBalanceByItemIdAsync(
            id,
            cancellationToken);

        var locationBalances =
            await _repository.GetLocationStockBalancesByItemIdAsync(
                id,
                cancellationToken);

        var stockTransactions =
            await _repository.GetStockTransactionsByItemIdAsync(
                id,
                cancellationToken);

        if (stockBalance is not null ||
            locationBalances.Count > 0 ||
            stockTransactions.Count > 0)
        {
            throw new InvalidOperationException(
                "This inventory item cannot be permanently deleted because " +
                "it has stock balance records or stock transaction history.");
        }

        await _repository.DeleteItemAsync(item, cancellationToken);

        await _repository.SaveChangesAsync(cancellationToken);

        return true;
    }





}
