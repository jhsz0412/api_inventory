using Identity.Application.DTOs.RolePermissions;
using Identity.Application.DTOs.Permissions;
using Identity.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Identity.Presentation.Controllers;

[ApiController]
[Route("api/identity/roles/{roleId}/permissions")]
public sealed class RolePermissionsController(
IRolePermissionService rolePermissionService) : ControllerBase
{
    [Authorize(Policy = "permission:identity.permissions.manage")]
    [HttpPost]
    [ProducesResponseType(
    typeof(IReadOnlyCollection<PermissionResponse>),
    StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<IReadOnlyCollection<PermissionResponse>>>
    Assign(
    Guid roleId,
    [FromBody] AssignPermissionToRoleRequest request,
    CancellationToken cancellationToken)
    {
        try
        {
            var permissions =
            await rolePermissionService.AssignPermissionAsync(
            roleId,
            request.PermissionId,
            cancellationToken);

            return Ok(permissions);
        }
        catch (ArgumentException exception)
        {
            return BadRequest(new { message = exception.Message });
        }
        catch (KeyNotFoundException exception)
        {
            return NotFound(new { message = exception.Message });
        }
        catch (InvalidOperationException exception)
        {
            return Conflict(new { message = exception.Message });
        }
    }

    [Authorize(Policy = "permission:identity.permissions.manage")]
    [HttpGet]
    [ProducesResponseType(
        typeof(IReadOnlyCollection<PermissionResponse>),
        StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<IReadOnlyCollection<PermissionResponse>>>
        GetPermissions(
            Guid roleId,
            CancellationToken cancellationToken)
    {
        try
        {
            var permissions =
                await rolePermissionService.GetPermissionsAsync(
                    roleId, cancellationToken);

            return Ok(permissions);
        }
        catch (ArgumentException exception)
        {
            return BadRequest(new { message = exception.Message });
        }
        catch (KeyNotFoundException exception)
        {
            return NotFound(new { message = exception.Message });
        }
    }

}
