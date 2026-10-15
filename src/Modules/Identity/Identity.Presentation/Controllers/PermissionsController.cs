using Identity.Application.DTOs.Permissions;
using Identity.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Identity.Presentation.Controllers;

[ApiController]
[Route("api/identity/permissions")]
public sealed class PermissionsController(
IPermissionService permissionService) : ControllerBase
{
    [Authorize(Policy = "permission:identity.permissions.manage")]
    [HttpPost]
    [ProducesResponseType(
    typeof(PermissionResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<PermissionResponse>> Create(
    [FromBody] CreatePermissionRequest request,
    CancellationToken cancellationToken)
    {
        try
        {
            var permission = await permissionService.CreateAsync(
            request, cancellationToken);

            return CreatedAtAction(
            nameof(GetById),
            new { id = permission.Id },
            permission);
        }
        catch (ArgumentException exception)
        {
            return BadRequest(new { message = exception.Message });
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
    public async Task<ActionResult<IReadOnlyCollection<PermissionResponse>>> GetAll(
        CancellationToken cancellationToken)
    {
        var permissions = await permissionService.GetAllAsync(
            cancellationToken);

        return Ok(permissions);
    }

    [Authorize(Policy = "permission:identity.permissions.manage")]
    [HttpGet("{id:guid}")]
    [ProducesResponseType(
        typeof(PermissionResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<PermissionResponse>> GetById(
        Guid id,
        CancellationToken cancellationToken)
    {
        var permission = await permissionService.GetByIdAsync(
            id, cancellationToken);

        return permission is null ? NotFound() : Ok(permission);
    }

}
