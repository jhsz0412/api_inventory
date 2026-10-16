using Approvals.Application.DTOs.Audit;
using Approvals.Application.Interfaces;

namespace Approvals.Application.Services;

public sealed class ApprovalAuditService : IApprovalAuditService
{
    private readonly IApprovalsRepository _repository;

    public ApprovalAuditService(IApprovalsRepository repository)
    {
        _repository = repository;
    }

    public async Task<IReadOnlyCollection<ApprovalAuditLogResponse>>
        GetByRequisitionIdAsync(
            Guid requisitionId,
            CancellationToken cancellationToken = default)
    {
        if (requisitionId == Guid.Empty)
            throw new ArgumentException("Requisition ID is required.");

        var logs = await _repository.GetAuditLogsByRequisitionIdAsync(
            requisitionId, cancellationToken);

        return logs.Select(x => new ApprovalAuditLogResponse
        {
            Id = x.Id,
            RequisitionId = x.RequisitionId,
            ActorEmployeeId = x.ActorEmployeeId,
            Action = x.Action,
            SequenceOrder = x.SequenceOrder,
            PreviousStatus = x.PreviousStatus,
            NewStatus = x.NewStatus,
            Comments = x.Comments,
            OccurredAtUtc = x.OccurredAtUtc
        }).ToList();
    }
}