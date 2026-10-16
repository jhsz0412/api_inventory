using Approvals.Application.DTOs.Approvals;
using Approvals.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Runtime.InteropServices;
using System.Security.Claims;

namespace Approvals.Presentation.Controllers;

[ApiController]
[Route("api/approvals/requisitions")]
[Authorize]
public sealed class RequisitionApprovalsController(
    IRequisitionApprovalService approvalService) : ControllerBase
{
    [HttpGet("{requisitionId:guid}")]
    [Authorize(Policy = "permission:requisitions.read")]
    public async Task<IActionResult> Get(
        Guid requisitionId, CancellationToken cancellationToken)
    {
        var result = await approvalService.GetByRequisitionIdAsync(
            requisitionId, cancellationToken);

        return result is null ? NotFound() : Ok(result);
    }

    [HttpPost("{requisitionId:guid}/approve")]
    [Authorize(Policy = "permission:requisitions.approve")]
    public async Task<IActionResult> Approve(
        Guid requisitionId,
        ApprovalActionRequest request,
        CancellationToken cancellationToken)
    {
        var actorId = GetEmployeeId();
        if (actorId is null) return Unauthorized();

        return Ok(await approvalService.ApproveAsync(
            requisitionId, actorId.Value, request, cancellationToken));
    }

    [HttpPost("{requisitionId:guid}/reject")]
    [Authorize(Policy = "permission:requisitions.reject")]
    public async Task<IActionResult> Reject(
        Guid requisitionId,
        ApprovalActionRequest request,
        CancellationToken cancellationToken)
    {
        var actorId = GetEmployeeId();
        if (actorId is null) return Unauthorized();

        return Ok(await approvalService.RejectAsync(
            requisitionId, actorId.Value, request, cancellationToken));
    }

    private Guid? GetEmployeeId()
    {
        var value = User.FindFirst("employeeId")?.Value;
        return Guid.TryParse(value, out var id) ? id : null;
    }
}
