using Delivery.Application.DTOs;
using Delivery.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Runtime.InteropServices;
using Microsoft.AspNetCore.Authorization;

namespace Delivery.Presentation.Controllers;

[ApiController]
[Route("api/deliveries")]
[Authorize(Policy = "permission:deliveries.read")]
public sealed class DeliveryController : ControllerBase
{
    private readonly IDeliveryService _deliveryService;
    private readonly IDeliveryDispatchWorkflow _dispatchWorkflow;

    public DeliveryController(
        IDeliveryService deliveryService,
        IDeliveryDispatchWorkflow dispatchWorkflow)
    {
        _deliveryService = deliveryService;
        _dispatchWorkflow = dispatchWorkflow;
    }

    [HttpPost]
    [Authorize(Policy = "permission:deliveries.create")]
    [ProducesResponseType(typeof(DeliveryResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<DeliveryResponse>> Create(
        [FromBody] CreateDeliveryRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            var delivery = await _deliveryService.CreateAsync(
                request,
                cancellationToken);

            return CreatedAtAction(
                nameof(GetById),
                new { id = delivery.Id },
                delivery);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpGet]
    [Authorize(Policy = "permission:deliveries.read")]
    [ProducesResponseType(typeof(IReadOnlyList<DeliveryResponse>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<DeliveryResponse>>> GetAll(
        CancellationToken cancellationToken)
    {
        var deliveries = await _deliveryService.GetAllAsync(cancellationToken);
        return Ok(deliveries);
    }

    [HttpGet("{id:guid}")]
    [Authorize(Policy = "permission:deliveries.read")]
    [ProducesResponseType(typeof(DeliveryResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<DeliveryResponse>> GetById(
        Guid id,
        CancellationToken cancellationToken)
    {
        var delivery = await _deliveryService.GetByIdAsync(id, cancellationToken);

        return delivery is null ? NotFound() : Ok(delivery);
    }

    [HttpPatch("{id:guid}/ready-for-dispatch")]
    [Authorize(Policy = "permission:deliveries.update")]
    [ProducesResponseType(typeof(DeliveryResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<DeliveryResponse>> MarkReadyForDispatch(
        Guid id,
        CancellationToken cancellationToken)
    {
        try
        {
            return Ok(await _deliveryService.MarkReadyForDispatchAsync(
                id,
                cancellationToken));
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
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPatch("{id:guid}/dispatch")]
    [Authorize(Policy = "permission:deliveries.dispatch")]
    [ProducesResponseType(typeof(DeliveryResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<DeliveryResponse>> Dispatch(
        Guid id,
        [FromQuery] Guid dispatchedByEmployeeId,
        CancellationToken cancellationToken)
    {
        try
        {
            return Ok(await _dispatchWorkflow.DispatchAsync(
                id,
                dispatchedByEmployeeId,
                cancellationToken));
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
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPost("{id:guid}/recover-requisition-fulfillment")]
    [Authorize(Policy = "permission:deliveries.recover")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> RecoverRequisitionFulfillment(
        Guid id,
        CancellationToken cancellationToken)
    {
        try
        {
            var recovered = await _dispatchWorkflow
                .RecoverRequisitionFulfillmentAsync(
                    id,
                    cancellationToken);

            if (!recovered)
            {
                return NotFound(new
                {
                    message = "The delivery's requisition was not found."
                });
            }

            return NoContent();
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

    [HttpPatch("{id:guid}/cancel")]
    [Authorize(Policy = "permission:deliver.cancel")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Cancel(
        Guid id,
        [FromQuery] string? remarks,
        CancellationToken cancellationToken)
    {
        try
        {
            await _deliveryService.CancelAsync(id, remarks, cancellationToken);
            return NoContent();
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
            return BadRequest(new { message = ex.Message });
        }
    }
}