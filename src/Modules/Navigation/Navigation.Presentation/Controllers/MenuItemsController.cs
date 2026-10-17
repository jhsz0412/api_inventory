
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Navigation.Application.DTOs;
using Navigation.Application.Interfaces;
using Navigation.Application.Services;
using System.Runtime.InteropServices;

namespace Navigation.Presentation.Controllers;

[ApiController]
[Route("api/menu-items")]
[Authorize]
public sealed class MenuItemsController(
    IMenuItemService menuItemService,
    IGroupMenuVisibilityService groupMenuVisibilityService,
    IGroupMenuAssignmentService groupMenuAssignmentService) : ControllerBase
{
    [HttpGet]
    [Authorize(Policy = "permission:navigation.menus.read")]
    public async Task<ActionResult<IReadOnlyList<MenuItemResponse>>> GetAll(
        CancellationToken cancellationToken)
    {
        if (!Guid.TryParse(
            User.FindFirst("employeeId")?.Value,
            out var employeeId))
        {
            return Unauthorized();
        }

        var menuItems = await groupMenuVisibilityService.GetVisibleMenuItemsAsync(
            employeeId,
            cancellationToken);

        return Ok(menuItems);
    }

    [HttpGet("{id:guid}")]
    [Authorize(Policy = "permission:navigation.menus.read")]
    public async Task<ActionResult<MenuItemResponse>> GetById(
        Guid id,
        CancellationToken cancellationToken)
    {
        var menuItem = await menuItemService.GetByIdAsync(
            id,
            cancellationToken);

        if (menuItem is null)
        {
            return NotFound();
        }

        return Ok(menuItem);
    }

    [HttpPost]
    [Authorize(Policy = "permission:navigation.menus.create")]
    public async Task<ActionResult<MenuItemResponse>> Create(
        CreateMenuItemRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            var menuItem = await menuItemService.CreateAsync(
                request,
                cancellationToken);

            return CreatedAtAction(
                nameof(GetById),
                new { id = menuItem.Id },
                menuItem);
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
    [Authorize(Policy = "permission:navigation.menus.update")]
    public async Task<ActionResult<MenuItemResponse>> Update(
        Guid id,
        UpdateMenuItemRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            var menuItem = await menuItemService.UpdateAsync(
                id,
                request,
                cancellationToken);

            if (menuItem is null)
            {
                return NotFound();
            }

            return Ok(menuItem);
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

    [HttpDelete("{id:guid}")]
    [Authorize(Policy = "permission:navigation.menus.delete")]
    public async Task<IActionResult> Deactivate(
        Guid id,
        CancellationToken cancellationToken)
    {
        var deactivated = await menuItemService.DeactivateAsync(
            id,
            cancellationToken);

        if (!deactivated)
        {
            return NotFound();
        }

        return NoContent();
    }


    [HttpGet("groups/{groupId:guid}")]
    [Authorize(Policy = "permission:navigation.menus.read")]
    public async Task<ActionResult<IReadOnlyList<MenuItemResponse>>> GetAssignedMenus(
        Guid groupId,
        CancellationToken cancellationToken)
    {
        try
        {
            var menuItems = await groupMenuAssignmentService.GetAssignedMenuItemsAsync(
                groupId,
                cancellationToken);

            return Ok(menuItems);
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

    [HttpPost("groups/{groupId:guid}/{menuItemId:guid}")]
    [Authorize(Policy = "permission:navigation.menus.update")]
    public async Task<IActionResult> AssignMenuToGroup(
        Guid groupId,
        Guid menuItemId,
        CancellationToken cancellationToken)
    {
        try
        {
            await groupMenuAssignmentService.AssignMenuAsync(
                groupId,
                menuItemId,
                cancellationToken);

            return NoContent();
        }
        catch (KeyNotFoundException exception)
        {
            return NotFound(new { message = exception.Message });
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

    [HttpDelete("groups/{groupId:guid}/{menuItemId:guid}")]
    [Authorize(Policy = "permission:navigation.menus.update")]
    public async Task<IActionResult> RemoveMenuFromGroup(
        Guid groupId,
        Guid menuItemId,
        CancellationToken cancellationToken)
    {
        try
        {
            var removed = await groupMenuAssignmentService.RemoveMenuAsync(
                groupId,
                menuItemId,
                cancellationToken);

            if (!removed)
            {
                return NotFound(new
                {
                    message = "The menu assignment was not found."
                });
            }

            return NoContent();
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


}
