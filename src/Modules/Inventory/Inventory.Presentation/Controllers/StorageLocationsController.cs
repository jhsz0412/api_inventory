using Inventory.Application.DTOs;
using Inventory.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Inventory.Presentation.Controllers;

[ApiController]
[Route("api/inventory/storage-locations")]
public sealed class StorageLocationsController : ControllerBase
{
    private readonly IInventoryService _inventoryService;

    public StorageLocationsController(
        IInventoryService inventoryService)
    {
        _inventoryService = inventoryService;
    }

    [HttpPost]
    [Authorize(Policy = "permission:inventory.storage-locations.create")]
    public async Task<IActionResult> Create(
        [FromBody] CreateStorageLocationRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            var location =
                await _inventoryService.CreateStorageLocationAsync(
                    request, cancellationToken);

            return CreatedAtAction(
                nameof(GetById),
                new { id = location.Id },
                location);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
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

    [HttpGet("warehouse/{warehouseId:guid}")]
    [Authorize(Policy = "permission:inventory.storage-locations.read")]
    public async Task<IActionResult> GetByWarehouse(
        Guid warehouseId,
        CancellationToken cancellationToken)
    {
        try
        {
            var locations =
                await _inventoryService
                    .GetStorageLocationsByWarehouseIdAsync(
                        warehouseId, cancellationToken);

            return Ok(locations);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }

    [HttpGet("{id:guid}")]
    [Authorize(Policy = "permission:inventory.storage-locations.read")]
    public async Task<IActionResult> GetById(
        Guid id,
        CancellationToken cancellationToken)
    {
        var location =
            await _inventoryService.GetStorageLocationByIdAsync(
                id, cancellationToken);

        return location is null ? NotFound() : Ok(location);
    }

    [HttpPut("{id:guid}")]
    [Authorize(Policy = "permission:inventory.storage-locations.update")]
    public async Task<IActionResult> Update(
        Guid id,
        [FromBody] UpdateStorageLocationRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            var location =
                await _inventoryService.UpdateStorageLocationAsync(
                    id, request, cancellationToken);

            return location is null ? NotFound() : Ok(location);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPatch("{id:guid}/status")]
    [Authorize(Policy = "permission:inventory.storage-locations.manage")]
    public async Task<IActionResult> SetActiveStatus(
        Guid id,
        [FromBody] SetStorageLocationStatusRequest request,
        CancellationToken cancellationToken)
    {
        var location =
            await _inventoryService.SetStorageLocationActiveStatusAsync(
                id, request.IsActive, cancellationToken);

        return location is null ? NotFound() : Ok(location);
    }
}

public sealed record SetStorageLocationStatusRequest(bool IsActive);