using Approvals.Application.DTOs.Workflows;
using Approvals.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Runtime.InteropServices;
using System.Security.Claims;

namespace Approvals.Presentation.Controllers;

[ApiController]
[Route("api/approvals/workflows")]
[Authorize]
public sealed class ApprovalWorkflowsController(
    IApprovalWorkflowService workflowService) : ControllerBase
{
    [HttpPost]
    [Authorize(Policy = "permission:approvals.workflows.manage")]
    public async Task<IActionResult> Create(
        CreateApprovalWorkflowRequest request,
        CancellationToken cancellationToken)
    {
        var actorId = GetEmployeeId();
        if (actorId is null) return Unauthorized();

        var result = await workflowService.CreateAsync(
            request, actorId.Value, cancellationToken);

        return CreatedAtAction(
            nameof(GetById), new { id = result.Id }, result);
    }

    [HttpGet("{id:guid}")]
    [Authorize(Policy = "permission:approvals.workflows.read")]
    public async Task<IActionResult> GetById(
        Guid id, CancellationToken cancellationToken)
    {
        var result = await workflowService.GetByIdAsync(id, cancellationToken);
        return result is null ? NotFound() : Ok(result);
    }

    [HttpGet("department/{departmentId:guid}")]
    [Authorize(Policy = "permission:approvals.workflows.read")]
    public async Task<IActionResult> GetByDepartment(
        Guid departmentId, CancellationToken cancellationToken)
    {
        return Ok(await workflowService.GetByDepartmentAsync(
            departmentId, cancellationToken));
    }

    [HttpPut("{id:guid}")]
    [Authorize(Policy = "permission:approvals.workflows.manage")]
    public async Task<IActionResult> Update(
        Guid id,
        UpdateApprovalWorkflowRequest request,
        CancellationToken cancellationToken)
    {
        var actorId = GetEmployeeId();
        if (actorId is null) return Unauthorized();

        return Ok(await workflowService.UpdateAsync(
            id, request, actorId.Value, cancellationToken));
    }

    [HttpPatch("{id:guid}/status")]
    [Authorize(Policy = "permission:approvals.workflows.manage")]
    public async Task<IActionResult> SetStatus(
        Guid id,
        SetApprovalWorkflowStatusRequest request,
        CancellationToken cancellationToken)
    {
        var actorId = GetEmployeeId();
        if (actorId is null) return Unauthorized();

        return Ok(await workflowService.SetStatusAsync(
            id, request, actorId.Value, cancellationToken));
    }

    private Guid? GetEmployeeId()
    {
        var value = User.FindFirst("employeeId")?.Value;
        return Guid.TryParse(value, out var id) ? id : null;
    }
}
