using Inventory.Application.DTOs;
using Inventory.Application.Interfaces;
using Inventory.Domain.Entities;
using Inventory.Application.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Inventory.Presentation.Controllers;

[ApiController]
[Route("api/inventory/location-stock")]
public sealed class InventoryLocationStockController : ControllerBase
{
    private readonly IInventoryService _inventoryService;

    public InventoryLocationStockController(
        IInventoryService inventoryService)
    {
        _inventoryService = inventoryService;
    }

    [HttpGet("{inventoryItemId:guid}/locations/{storageLocationId:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetLocationStockBalance(
        Guid inventoryItemId,
        Guid storageLocationId,
        CancellationToken cancellationToken)
    {
        try
        {
            var balance = await _inventoryService.GetLocationStockBalanceAsync(
                inventoryItemId,
                storageLocationId,
                cancellationToken);

            if (balance is null)
            {
                return NotFound(new
                {
                    message = "No stock balance exists for this item at this location."
                });
            }

            return Ok(balance);
        }
        catch (ArgumentException exception)
        {
            return BadRequest(new { message = exception.Message });
        }
    }

    [HttpGet("{inventoryItemId:guid}/locations")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetLocationStockBalancesByItem(
        Guid inventoryItemId,
        CancellationToken cancellationToken)
    {
        try
        {
            var balances =
                await _inventoryService.GetLocationStockBalancesByItemIdAsync(
                    inventoryItemId,
                    cancellationToken);

            return Ok(balances);
        }
        catch (ArgumentException exception)
        {
            return BadRequest(new { message = exception.Message });
        }
    }
    [HttpPost("receive")]
    [ProducesResponseType(typeof(InventoryLocationStockBalance), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ReceiveStockAtLocation(
        [FromBody] ReceiveStockAtLocationRequest request,
        CancellationToken cancellationToken)
        {
            try
            {
                var balance = await _inventoryService.ReceiveStockAtLocationAsync(
                    request,
                    cancellationToken);

                return Ok(balance);
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
                return BadRequest(new { message = ex.Message });
            }
    }
    [HttpPost("issue")]
    [ProducesResponseType(typeof(InventoryLocationStockBalance), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> IssueStockAtLocation(
        [FromBody] IssueStockAtLocationRequest request,
        CancellationToken cancellationToken)
        {
            try
            {
                var balance = await _inventoryService.IssueStockAtLocationAsync(
                    request,
                    cancellationToken);

                return Ok(balance);
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
                return BadRequest(new { message = ex.Message });
            }
    }

    [HttpPost("transfer")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> TransferStockBetweenLocations(
        [FromBody] TransferStockRequest request,
        CancellationToken cancellationToken)
        {
            try
            {
                await _inventoryService.TransferStockBetweenLocationsAsync(
                    request,
                    cancellationToken);

                return NoContent();
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
                return BadRequest(new { message = ex.Message });
            }
    }

    [HttpGet("{inventoryItemId:guid}/locations/{storageLocationId:guid}/transactions")]
    [ProducesResponseType(
        typeof(IReadOnlyCollection<InventoryStockTransaction>),
        StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetLocationStockTransactions(
        Guid inventoryItemId,
        Guid storageLocationId,
        CancellationToken cancellationToken)
        {
            try
            {
                var transactions =
                    await _inventoryService.GetLocationStockTransactionsAsync(
                        inventoryItemId,
                        storageLocationId,
                        cancellationToken);

                return Ok(transactions);
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

    [HttpGet("{inventoryItemId:guid}/summary")]
    [ProducesResponseType(
        typeof(InventoryLocationStockSummaryResponse),
        StatusCodes.Status200OK)]
    public async Task<ActionResult<InventoryLocationStockSummaryResponse>>
        GetInventoryLocationStockSummary(
            Guid inventoryItemId,
            CancellationToken cancellationToken)
        {
            var summary = await _inventoryService
                .GetInventoryLocationStockSummaryAsync(
                    inventoryItemId,
                    cancellationToken);

            return Ok(summary);
    }

    [HttpPost("adjust")]
    [ProducesResponseType(
        typeof(InventoryLocationStockBalance),
        StatusCodes.Status200OK)]
    public async Task<ActionResult<InventoryLocationStockBalance>>
        AdjustStockAtLocation(
            [FromBody] AdjustStockAtLocationRequest request,
            CancellationToken cancellationToken)
        {
            var balance = await _inventoryService
                .AdjustStockAtLocationAsync(
                    request,
                    cancellationToken);

            return Ok(balance);
        }
}

