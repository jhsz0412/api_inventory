using Inventory.Application.DTOs;
using Inventory.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Inventory.Presentation.Controllers;

[ApiController]
[Route("api/inventory/stock")]
public sealed class InventoryStockController : ControllerBase
{
    private readonly IInventoryService _inventoryService;

    public InventoryStockController(IInventoryService inventoryService)
    {
        _inventoryService = inventoryService;
    }

    [Authorize(Policy = "permission:inventory.stock.read")]
    [HttpGet("{inventoryItemId:guid}")]
    public async Task<IActionResult> GetStockBalance(
        Guid inventoryItemId,
        CancellationToken cancellationToken)
    {
        var balance = await _inventoryService.GetStockBalanceAsync(
            inventoryItemId,
            cancellationToken);

        return balance is null ? NotFound() : Ok(balance);
    }

    [Authorize(Policy = "permission:inventory.stock.receive")]
    [HttpPost("{inventoryItemId:guid}/receive")]
    public async Task<IActionResult> ReceiveStock(
        Guid inventoryItemId,
        [FromBody] StockQuantityRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            var balance = await _inventoryService.ReceiveStockAsync(
                inventoryItemId,
                request.Quantity,
                cancellationToken);

            return Ok(balance);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (ArgumentOutOfRangeException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [Authorize(Policy = "permission:inventory.stock.issue")]
    [HttpPost("{inventoryItemId:guid}/issue")]
    public async Task<IActionResult> IssueStock(
        Guid inventoryItemId,
        [FromBody] StockQuantityRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            var balance = await _inventoryService.IssueStockAsync(
                inventoryItemId,
                request.Quantity,
                cancellationToken);

            return Ok(balance);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (ArgumentOutOfRangeException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new { message = ex.Message });
        }
    }

    [Authorize(Policy = "permission:inventory.stock.read")]
    [HttpGet("{inventoryItemId:guid}/transactions")]
    public async Task<IActionResult> GetStockTransactions(
    Guid inventoryItemId,
    CancellationToken cancellationToken)
    {
        try
        {
            var transactions =
                await _inventoryService.GetStockTransactionsAsync(
                    inventoryItemId,
                    cancellationToken);

            return Ok(transactions);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }

    [Authorize(Policy = "permission:inventory.stock.adjust")]
    [HttpPost("{inventoryItemId:guid}/adjust")]
    public async Task<IActionResult> AdjustStock(
        Guid inventoryItemId,
        [FromBody] AdjustInventoryStockRequest request,
        CancellationToken cancellationToken)
        {
            try
            {
                var balance = await _inventoryService.AdjustStockAsync(
                    inventoryItemId,
                    request,
                    cancellationToken);

                return Ok(balance);
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
}

public sealed record StockQuantityRequest(decimal Quantity);