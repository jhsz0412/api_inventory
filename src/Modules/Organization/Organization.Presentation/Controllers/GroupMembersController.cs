
using Microsoft.AspNetCore.Mvc;
using Organization.Application.DTOs.GroupMembers;
using Organization.Application.Services.GroupMembers;

namespace Organization.Presentation.Controllers;

[ApiController]
[Route("api/groups/{groupId:guid}/members")]
public sealed class GroupMembersController(
    IGroupMemberService service) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetMembers(
        Guid groupId,
        CancellationToken cancellationToken)
    {
        try
        {
            return Ok(await service.GetMembersAsync(
                groupId, cancellationToken));
        }
        catch (KeyNotFoundException exception)
        {
            return NotFound(new { message = exception.Message });
        }
    }

    [HttpPost]
    public async Task<IActionResult> AddMembers(
        Guid groupId,
        [FromBody] AddGroupMembersRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            var count = await service.AddMembersAsync(
                groupId, request, cancellationToken);

            return Ok(new
            {
                message = "Group membership processed.",
                eligibleEmployees = count
            });
        }
        catch (KeyNotFoundException exception)
        {
            return NotFound(new { message = exception.Message });
        }
        catch (ArgumentException exception)
        {
            return BadRequest(new { message = exception.Message });
        }
    }

    [HttpPost("copy-from/{sourceGroupId:guid}")]
    public async Task<IActionResult> CopyMembers(
        Guid groupId,
        Guid sourceGroupId,
        CancellationToken cancellationToken)
    {
        try
        {
            var count = await service.CopyMembersAsync(
                groupId, sourceGroupId, cancellationToken);

            return Ok(new
            {
                message = "Group membership copied.",
                sourceMembers = count
            });
        }
        catch (KeyNotFoundException exception)
        {
            return NotFound(new { message = exception.Message });
        }
        catch (ArgumentException exception)
        {
            return BadRequest(new { message = exception.Message });
        }
    }

    [HttpDelete("{employeeId:guid}")]
    public async Task<IActionResult> RemoveMember(
        Guid groupId,
        Guid employeeId,
        CancellationToken cancellationToken)
    {
        try
        {
            var removed = await service.RemoveMemberAsync(
                groupId, employeeId, cancellationToken);

            return removed
                ? NoContent()
                : NotFound(new
                {
                    message = "This employee is not a member of the group."
                });
        }
        catch (KeyNotFoundException exception)
        {
            return NotFound(new { message = exception.Message });
        }
    }
}
