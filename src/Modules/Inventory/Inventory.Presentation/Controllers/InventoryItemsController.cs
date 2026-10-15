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
}