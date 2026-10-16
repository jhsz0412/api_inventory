using Inventory.Application.DTOs;
using Inventory.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Inventory.Presentation.Controllers;

[ApiController]
[Route("api/inventory/items")]
public sealed class InventoryItemsController : ControllerBase
{
    private readonly IInventoryService _inventoryService;

    public InventoryItemsController(IInventoryService inventoryService)
    {
        _inventoryService = inventoryService;
    }

    [Authorize(Policy = "permission:inventory.items.read")]
    [HttpGet]
    public async Task<IActionResult> GetItems(
        CancellationToken cancellationToken)
    {
        var items = await _inventoryService.GetItemsAsync(cancellationToken);
        return Ok(items);
    }

    [Authorize(Policy = "permission:inventory.items.read")]
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetItemById(
        Guid id,
        CancellationToken cancellationToken)
    {
        var item = await _inventoryService.GetItemByIdAsync(
            id,
            cancellationToken);

        return item is null ? NotFound() : Ok(item);
    }

    [Authorize(Policy = "permission:inventory.items.create")]
    [HttpPost]
    public async Task<IActionResult> CreateItem(
        [FromBody] CreateInventoryItemRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            var item = await _inventoryService.CreateItemAsync(
                request,
                cancellationToken);

            return CreatedAtAction(
                nameof(GetItemById),
                new { id = item.Id },
                item);
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new { message = ex.Message });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }



    [Authorize(Policy = "permission:inventory.items.update")]
    [HttpPut("{id:guid}")]
    public async Task<IActionResult> UpdateItem(
        Guid id,
        [FromBody] UpdateInventoryItemRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            var item = await _inventoryService.UpdateItemAsync(
                id,
                request,
                cancellationToken);

            return item is null ? NotFound() : Ok(item);
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new { message = ex.Message });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (KeyNotFoundException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [Authorize(Policy = "permission:inventory.items.delete")]
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteItem(
    Guid id,
    CancellationToken cancellationToken)
    {
        try
        {
            var deleted = await _inventoryService.DeactivateItemAsync(
                id,
                cancellationToken);

            return deleted ? NoContent() : NotFound();
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new { message = ex.Message });
        }
    }


    [Authorize(Policy = "permission:inventory.items.restore")]
    [HttpPut("{id:guid}/restore")]
    public async Task<IActionResult> RestoreItem(
        Guid id,
        CancellationToken cancellationToken)
    {
        try
        {
            var restored = await _inventoryService.RestoreItemAsync(
                id,
                cancellationToken);

            return restored
                ? NoContent()
                : NotFound();
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new { message = ex.Message });
        }
    }


    [Authorize(Policy = "permission:inventory.items.delete-permanent")]
    [HttpDelete("{id:guid}/permanent")]
    public async Task<IActionResult> PermanentlyDeleteItem(
        Guid id,
        CancellationToken cancellationToken)
    {
        try
        {
            var deleted =
                await _inventoryService.PermanentlyDeleteItemAsync(
                    id,
                    cancellationToken);

            return deleted ? NoContent() : NotFound();
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new { message = ex.Message });
        }
    }




}