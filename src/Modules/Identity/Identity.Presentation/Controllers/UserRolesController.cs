using Identity.Application.DTOs.UserRoles;
using Identity.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Identity.Presentation.Controllers;

[ApiController]
[Route("api/identity/users/{userId}/roles")]
public sealed class UserRolesController(
IUserRoleService userRoleService) : ControllerBase
{
    [Authorize(Policy = "permission:identity.users.roles.manage")]
    [HttpPost]
    public async Task<IActionResult> AssignRole(
    Guid userId,
    [FromBody] AssignRoleToUserRequest request,
    CancellationToken cancellationToken)
    {
        try
        {
            var roles = await userRoleService.AssignRoleAsync(
            userId,
            request.RoleId,
            cancellationToken);

            return Ok(roles);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new { message = ex.Message });
        }
    }

    [Authorize(Policy = "permission:identity.users.roles.manage")]
    [HttpGet]
    public async Task<IActionResult> GetRoles(
        Guid userId,
        CancellationToken cancellationToken)
    {
        try
        {
            var roles = await userRoleService.GetRolesForUserAsync(
                userId,
                cancellationToken);

            return Ok(roles);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }

}
