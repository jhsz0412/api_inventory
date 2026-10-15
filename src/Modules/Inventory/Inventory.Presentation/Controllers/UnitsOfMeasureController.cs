using Inventory.Application.DTOs;
using Inventory.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Inventory.Presentation.Controllers;

[ApiController]
[Route("api/inventory/units-of-measure")]
public sealed class UnitsOfMeasureController : ControllerBase
{
    private readonly IInventoryService _inventoryService;

    public UnitsOfMeasureController(IInventoryService inventoryService)
    {
        _inventoryService = inventoryService;
    }

    [HttpGet]
    [Authorize(Policy = "permission:inventory.units.read")]
    public async Task<IActionResult> GetUnitsOfMeasure(
        CancellationToken cancellationToken)
    {
        var units = await _inventoryService.GetUnitsOfMeasureAsync(
            cancellationToken);

        return Ok(units);
    }

    [HttpPost]
    [Authorize(Policy = "permission:inventory.units.create")]
    public async Task<IActionResult> CreateUnitOfMeasure(
        [FromBody] CreateUnitOfMeasureRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            var unit = await _inventoryService.CreateUnitOfMeasureAsync(
                request,
                cancellationToken);

            return CreatedAtAction(
                nameof(GetUnitsOfMeasure),
                new { id = unit.Id },
                unit);
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