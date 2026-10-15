using Inventory.Application.DTOs;
using Inventory.Application.Interfaces;
using Inventory.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Inventory.Presentation.Controllers;

[ApiController]
[Route("api/inventory/categories")]
public sealed class InventoryCategoriesController : ControllerBase
{
    private readonly IInventoryService _inventoryService;

    public InventoryCategoriesController(IInventoryService inventoryService)
    {
        _inventoryService = inventoryService;
    }

    [Authorize(Policy = "permission:inventory.categories.read")]
    [HttpGet]
    public async Task<IActionResult> GetCategories(
        CancellationToken cancellationToken)
    {
        var categories = await _inventoryService.GetCategoriesAsync(
            cancellationToken);

        return Ok(categories);
    }

    [Authorize(Policy = "permission:inventory.categories.create")]
    [HttpPost]
    public async Task<IActionResult> CreateCategory(
        [FromBody] CreateInventoryCategoryRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            var category = await _inventoryService.CreateCategoryAsync(
                request,
                cancellationToken);

            return CreatedAtAction(
                nameof(GetCategories),
                new { id = category.Id },
                category);
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