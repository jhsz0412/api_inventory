
using Approvals.Application.DTOs.Approvals;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace InventoryManagement.Api.Integrations;

[ApiController]
[Route("api/requisitions")]
[Authorize]
public sealed class RequisitionApprovalController(
    RequisitionApprovalCoordinator coordinator) : ControllerBase
{
    [HttpPost("{id:guid}/submit")]
    [Authorize(Policy = "permission:requisitions.submit")]
    public async Task<IActionResult> Submit(
        Guid id,
        CancellationToken cancellationToken)
    {
        var actorId = GetEmployeeId();

        if (actorId is null)
            return Unauthorized();

        try
        {
            var result = await coordinator.SubmitAsync(
                id, actorId.Value, cancellationToken);

            return result is null ? NotFound() : Ok(result);
        }
        catch (KeyNotFoundException exception)
        {
            return NotFound(new { message = exception.Message });
        }
        catch (InvalidOperationException exception)
        {
            return BadRequest(new { message = exception.Message });
        }
    }

    [HttpPost("{id:guid}/approve")]
    [Authorize(Policy = "permission:requisitions.approve")]
    public async Task<IActionResult> Approve(
        Guid id,
        [FromBody] ApprovalActionRequest request,
        CancellationToken cancellationToken)
    {
        var actorId = GetEmployeeId();

        if (actorId is null)
            return Unauthorized();

        try
        {
            var result = await coordinator.ApproveAsync(
                id, actorId.Value, request, cancellationToken);

            return Ok(result);
        }
        catch (KeyNotFoundException exception)
        {
            return NotFound(new { message = exception.Message });
        }
        catch (InvalidOperationException exception)
        {
            return BadRequest(new { message = exception.Message });
        }
    }

    [HttpPost("{id:guid}/reject")]
    [Authorize(Policy = "permission:requisitions.reject")]
    public async Task<IActionResult> Reject(
        Guid id,
        [FromBody] ApprovalActionRequest request,
        CancellationToken cancellationToken)
    {
        var actorId = GetEmployeeId();

        if (actorId is null)
            return Unauthorized();

        try
        {
            var result = await coordinator.RejectAsync(
                id, actorId.Value, request, cancellationToken);

            return Ok(result);
        }
        catch (KeyNotFoundException exception)
        {
            return NotFound(new { message = exception.Message });
        }
        catch (InvalidOperationException exception)
        {
            return BadRequest(new { message = exception.Message });
        }
    }

    private Guid? GetEmployeeId()
    {
        var value = User.FindFirst("employeeId")?.Value;

        return Guid.TryParse(value, out var id) ? id : null;
    }
}
