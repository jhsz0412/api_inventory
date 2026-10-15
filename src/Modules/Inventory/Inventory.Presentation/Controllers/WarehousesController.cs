using Inventory.Application.DTOs;
using Inventory.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Inventory.Presentation.Controllers;

[ApiController]
[Route("api/inventory/warehouses")]
public sealed class WarehousesController : ControllerBase
{
    private readonly IInventoryService _inventoryService;

    public WarehousesController(IInventoryService inventoryService)
    {
        _inventoryService = inventoryService;
    }

    [HttpPost]
    [Authorize(Policy = "permission:inventory.warehouses.create")]
    public async Task<IActionResult> Create(
        [FromBody] CreateWarehouseRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            var warehouse =
                await _inventoryService.CreateWarehouseAsync(
                    request, cancellationToken);

            return CreatedAtAction(
                nameof(GetById),
                new { id = warehouse.Id },
                warehouse);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new { message = ex.Message });
        }
    }

    [HttpGet]
    [Authorize(Policy = "permission:inventory.warehouses.read")]
    public async Task<IActionResult> GetAll(
        CancellationToken cancellationToken)
    {
        var warehouses =
            await _inventoryService.GetWarehousesAsync(
                cancellationToken);

        return Ok(warehouses);
    }

    [HttpGet("{id:guid}")]
    [Authorize(Policy = "permission:inventory.warehouses.read")]
    public async Task<IActionResult> GetById(
        Guid id,
        CancellationToken cancellationToken)
    {
        var warehouse =
            await _inventoryService.GetWarehouseByIdAsync(
                id, cancellationToken);

        return warehouse is null ? NotFound() : Ok(warehouse);
    }

    [HttpPut("{id:guid}")]
    [Authorize(Policy = "permission:inventory.warehouses.update")]
    public async Task<IActionResult> Update(
        Guid id,
        [FromBody] UpdateWarehouseRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            var warehouse =
                await _inventoryService.UpdateWarehouseAsync(
                    id, request, cancellationToken);

            return warehouse is null ? NotFound() : Ok(warehouse);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPatch("{id:guid}/status")]
    [Authorize(Policy = "permission:inventory.warehouses.update")]
    public async Task<IActionResult> SetActiveStatus(
        Guid id,
        [FromBody] SetWarehouseStatusRequest request,
        CancellationToken cancellationToken)
    {
        var warehouse =
            await _inventoryService.SetWarehouseActiveStatusAsync(
                id, request.IsActive, cancellationToken);

        return warehouse is null ? NotFound() : Ok(warehouse);
    }
}

public sealed record SetWarehouseStatusRequest(bool IsActive);