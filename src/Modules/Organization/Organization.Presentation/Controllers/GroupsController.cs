using Microsoft.AspNetCore.Mvc;
using Organization.Application.DTOs.Groups;
using Organization.Application.Services.Groups;

namespace Organization.Presentation.Controllers;

[ApiController]
[Route("api/groups")]
public sealed class GroupsController(
    IGroupService groupService) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAll(
        CancellationToken cancellationToken)
    {
        var groups = await groupService.GetAllAsync(cancellationToken);
        return Ok(groups);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(
        Guid id,
        CancellationToken cancellationToken)
    {
        var group = await groupService.GetByIdAsync(
            id, cancellationToken);

        return group is null ? NotFound() : Ok(group);
    }

    [HttpPost]
    public async Task<IActionResult> Create(
        [FromBody] CreateGroupRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            var group = await groupService.CreateAsync(
                request, cancellationToken);

            return CreatedAtAction(
                nameof(GetById),
                new { id = group.Id },
                group);
        }
        catch (InvalidOperationException exception)
        {
            return Conflict(new { message = exception.Message });
        }
        catch (ArgumentException exception)
        {
            return BadRequest(new { message = exception.Message });
        }
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(
        Guid id,
        [FromBody] UpdateGroupRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            var group = await groupService.UpdateAsync(
                id, request, cancellationToken);

            return group is null ? NotFound() : Ok(group);
        }
        catch (InvalidOperationException exception)
        {
            return Conflict(new { message = exception.Message });
        }
        catch (ArgumentException exception)
        {
            return BadRequest(new { message = exception.Message });
        }
    }

    [HttpPatch("{id:guid}/status")]
    public async Task<IActionResult> UpdateStatus(
        Guid id,
        [FromBody] UpdateGroupStatusRequest request,
        CancellationToken cancellationToken)
    {
        var updated = await groupService.SetStatusAsync(
            id, request.IsActive, cancellationToken);

        return updated ? NoContent() : NotFound();
    }

    public sealed record UpdateGroupStatusRequest(bool IsActive);
}