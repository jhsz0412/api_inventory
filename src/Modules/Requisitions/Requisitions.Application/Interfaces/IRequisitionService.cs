using Requisitions.Application.DTOs;

namespace Requisitions.Application.Interfaces;

public interface IRequisitionService
{
    Task<RequisitionResponse> CreateAsync(
        CreateRequisitionRequest request,
        CancellationToken cancellationToken = default);

    Task<RequisitionResponse?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyCollection<RequisitionResponse>> GetAllAsync(
        CancellationToken cancellationToken = default);

    Task<RequisitionResponse?> AddItemAsync(
        Guid requisitionId,
        AddRequisitionItemRequest request,
        CancellationToken cancellationToken = default);

    Task<RequisitionResponse?> UpdateItemAsync(
        Guid requisitionId,
        Guid requisitionItemId,
        UpdateRequisitionItemRequest request,
        CancellationToken cancellationToken = default);

    Task<RequisitionResponse?> RemoveItemAsync(
        Guid requisitionId,
        Guid requisitionItemId,
        CancellationToken cancellationToken = default);

    Task<RequisitionResponse?> SubmitAsync(
        Guid requisitionId,
        CancellationToken cancellationToken = default);

    Task<RequisitionResponse?> ApproveAsync(
        Guid requisitionId,
        CancellationToken cancellationToken = default);

    Task<RequisitionResponse?> RejectAsync(
        Guid requisitionId,
        CancellationToken cancellationToken = default);

    Task<RequisitionResponse?> CancelAsync(
        Guid requisitionId,
        CancellationToken cancellationToken = default);


    Task<RequisitionResponse?> FulfillAsync(
        Guid requisitionId,
        CancellationToken cancellationToken = default);

    Task<RequisitionResponse?> MarkFulfilledAfterDeliveryAsync(
        Guid requisitionId,
        CancellationToken cancellationToken = default);

}
