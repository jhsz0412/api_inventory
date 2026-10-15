using Requisitions.Application.DTOs;
using Requisitions.Application.Interfaces;
using Requisitions.Domain.Entities;
using Requisitions.Domain.Enums;

namespace Requisitions.Application.Services;

public sealed class RequisitionService : IRequisitionService
{
    private readonly IRequisitionRepository _repository;
    private readonly IRequisitionInventoryService _inventoryService;

    public RequisitionService(
        IRequisitionRepository repository,
        IRequisitionInventoryService inventoryService)
    {
        _repository = repository;
        _inventoryService = inventoryService;
    }

    public async Task<RequisitionResponse> CreateAsync(
        CreateRequisitionRequest request,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        var requisition = new Requisition(
            request.RequestedByEmployeeId,
            request.DepartmentId,
            request.FiscalYearId,
            request.Purpose);

        var now = DateTime.UtcNow;
        var sequence = await _repository.GetNextMonthlySequenceAsync(
            now.Year,
            now.Month,
            cancellationToken);

        var requisitionNumber =
            $"REQ-{now:yyyyMM}-{sequence:D4}";

        requisition.AssignRequisitionNumber(requisitionNumber);

        await _repository.AddAsync(requisition, cancellationToken);
        await _repository.SaveChangesAsync(cancellationToken);

        return MapToResponse(requisition);
    }

    public async Task<RequisitionResponse?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        var requisition = await _repository.GetByIdAsync(
            id,
            cancellationToken);

        return requisition is null
            ? null
            : MapToResponse(requisition);
    }

    public async Task<IReadOnlyCollection<RequisitionResponse>> GetAllAsync(
        CancellationToken cancellationToken = default)
    {
        var requisitions = await _repository.GetAllAsync(
            cancellationToken);

        return requisitions
            .Select(MapToResponse)
            .ToArray();
    }

    private static RequisitionResponse MapToResponse(
        Requisition requisition)
    {
        return new RequisitionResponse(
            requisition.Id,
            requisition.RequisitionNumber,
            requisition.RequestedByEmployeeId,
            requisition.DepartmentId,
            requisition.FiscalYearId,
            requisition.RequestedAtUtc,
            requisition.Purpose,
            requisition.Status.ToString(),
            requisition.Items
                .Select(item => new RequisitionItemResponse(
                    item.Id,
                    item.InventoryItemId,
                    item.QuantityRequested,
                    item.Notes))
                .ToArray());
    }

    public async Task<RequisitionResponse?> AddItemAsync(
        Guid requisitionId,
        AddRequisitionItemRequest request,
        CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(request);

            var requisition = await _repository.GetByIdAsync(
                requisitionId,
                cancellationToken);

            if (requisition is null)
                return null;

            if (requisition.Status != RequisitionStatus.Draft)
                throw new InvalidOperationException(
                        "Items can only be added to draft requisitions.");

            var item = new RequisitionItem(
                requisition.Id,
                request.InventoryItemId,
                request.QuantityRequested,
                request.Notes);

            requisition.AddItem(item);

            await _repository.AddItemAsync(item, cancellationToken);
            await _repository.SaveChangesAsync(cancellationToken);

        return MapToResponse(requisition);
    }

    public async Task<RequisitionResponse?> UpdateItemAsync(
        Guid requisitionId,
        Guid requisitionItemId,
        UpdateRequisitionItemRequest request,
        CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(request);

            var requisition = await _repository.GetByIdAsync(
                requisitionId,
                cancellationToken);

            if (requisition is null)
                return null;

            if (requisition.Status != RequisitionStatus.Draft)
                throw new InvalidOperationException(
                    "Items can only be updated in draft requisitions.");

            var item = requisition.Items.FirstOrDefault(
                existing => existing.Id == requisitionItemId);

            if (item is null)
                throw new KeyNotFoundException(
                    "The requisition item was not found.");

            item.ChangeQuantity(request.QuantityRequested);
            item.UpdateNotes(request.Notes);

            await _repository.SaveChangesAsync(cancellationToken);

            return MapToResponse(requisition);
    }

    public async Task<RequisitionResponse?> RemoveItemAsync(
        Guid requisitionId,
        Guid requisitionItemId,
        CancellationToken cancellationToken = default)
        {
            var requisition = await _repository.GetByIdAsync(
                requisitionId,
                cancellationToken);

            if (requisition is null)
                return null;

            if (requisition.Status != RequisitionStatus.Draft)
                throw new InvalidOperationException(
                    "Items can only be removed from draft requisitions.");

            requisition.RemoveItem(requisitionItemId);

            await _repository.SaveChangesAsync(cancellationToken);

            return MapToResponse(requisition);
    }

    public async Task<RequisitionResponse?> SubmitAsync(
        Guid requisitionId,
        CancellationToken cancellationToken = default)
        {
            var requisition = await _repository.GetByIdAsync(
                requisitionId,
                cancellationToken);

            if (requisition is null)
                return null;

            requisition.Submit();

            await _repository.SaveChangesAsync(cancellationToken);

            return MapToResponse(requisition);
    }

    public async Task<RequisitionResponse?> ApproveAsync(
        Guid requisitionId,
        CancellationToken cancellationToken = default)
        {
            var requisition = await _repository.GetByIdAsync(
                requisitionId,
                cancellationToken);

            if (requisition is null)
                return null;

            requisition.Approve();

            await _repository.SaveChangesAsync(cancellationToken);

            return MapToResponse(requisition);
        }


    public async Task<RequisitionResponse?> RejectAsync(
        Guid requisitionId,
        CancellationToken cancellationToken = default)
    {
        var requisition = await _repository.GetByIdAsync(
            requisitionId,
            cancellationToken);

        if (requisition is null)
            return null;

        requisition.Reject();

        await _repository.SaveChangesAsync(cancellationToken);

        return MapToResponse(requisition);
    }


    public async Task<RequisitionResponse?> CancelAsync(
        Guid requisitionId,
        CancellationToken cancellationToken = default)
    {
        var requisition = await _repository.GetByIdAsync(
            requisitionId,
            cancellationToken);

        if (requisition is null)
            return null;

        requisition.Cancel();

        await _repository.SaveChangesAsync(cancellationToken);

        return MapToResponse(requisition);
    }





    public async Task<RequisitionResponse?> FulfillAsync(
        Guid requisitionId,
        CancellationToken cancellationToken = default)
    {
        var requisition = await _repository.GetByIdAsync(
            requisitionId,
            cancellationToken);

        if (requisition is null)
            return null;

        if (requisition.Status != RequisitionStatus.Approved &&
            requisition.Status != RequisitionStatus.FulfillmentInProgress)
        {
            throw new InvalidOperationException(
                "Only approved requisitions or requisitions already in progress " +
                "can be fulfilled.");
        }

        if (requisition.Items.Count == 0)
            throw new InvalidOperationException(
                "A requisition must contain at least one item to be fulfilled.");

        var requestedItems = requisition.Items
            .GroupBy(item => item.InventoryItemId)
            .Select(group => new RequisitionStockIssueRequest(
                group.Key,
                group.Sum(item => item.QuantityRequested)))
            .ToArray();

        // Only validate stock and claim the requisition when starting.
        if (requisition.Status == RequisitionStatus.Approved)
        {
            foreach (var item in requestedItems)
            {
                var availableQuantity =
                    await _inventoryService.GetAvailableQuantityAsync(
                        item.InventoryItemId,
                        cancellationToken);

                if (availableQuantity is null)
                    throw new KeyNotFoundException(
                        $"Inventory item '{item.InventoryItemId}' was not found.");

                if (availableQuantity.Value < item.Quantity)
                    throw new InvalidOperationException(
                        $"Insufficient stock for inventory item '{item.InventoryItemId}'. " +
                        $"Requested: {item.Quantity}, Available: {availableQuantity.Value}.");
            }

            requisition.BeginFulfillment();

            // Save the claim before attempting stock issuance.
            await _repository.SaveChangesAsync(cancellationToken);
        }

        // The inventory service checks for existing stock-out transactions.
        // Matching transactions must not cause another stock deduction.
        await _inventoryService.IssueStockBatchAsync(
            requestedItems,
            requisition.RequisitionNumber,
            cancellationToken);

        // Stock issuance may already have committed, so do not restore Approved
        // if the final requisition save fails.
        requisition.MarkFulfilled();

        await _repository.SaveChangesAsync(cancellationToken);

        return MapToResponse(requisition);
    }

    public async Task<RequisitionResponse?> MarkFulfilledAfterDeliveryAsync(
        Guid requisitionId,
        CancellationToken cancellationToken = default)
        {
            var requisition = await _repository.GetByIdAsync(
                requisitionId,
                cancellationToken);

            if (requisition is null)
                return null;

            // Recovery is idempotent: already fulfilled means success.
            // Do not save or issue inventory again.
            if (requisition.Status == RequisitionStatus.Fulfilled)
                return MapToResponse(requisition);

            if (requisition.Status != RequisitionStatus.Approved &&
                requisition.Status != RequisitionStatus.FulfillmentInProgress)
            {
                throw new InvalidOperationException(
                    "Only approved requisitions or requisitions already in progress " +
                    "can be fulfilled after delivery.");
            }

            if (requisition.Items.Count == 0)
                throw new InvalidOperationException(
                    "A requisition must contain at least one item to be fulfilled.");

            if (requisition.Status == RequisitionStatus.Approved)
                requisition.BeginFulfillment();

            requisition.MarkFulfilled();

            await _repository.SaveChangesAsync(cancellationToken);

            return MapToResponse(requisition);
        }




}