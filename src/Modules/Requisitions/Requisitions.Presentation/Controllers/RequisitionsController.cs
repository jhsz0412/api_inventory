using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Requisitions.Application.DTOs;
using Requisitions.Application.Interfaces;

namespace Requisitions.Presentation.Controllers;

[ApiController]
[Route("api/requisitions")]
public sealed class RequisitionsController : ControllerBase
{
    private readonly IRequisitionService _service;

    public RequisitionsController(IRequisitionService service)
    {
        _service = service;
    }

    [HttpPost]
    [Authorize(Policy = "permission:requisitions.create")]
    [ProducesResponseType(typeof(RequisitionResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<RequisitionResponse>> Create(
        [FromBody] CreateRequisitionRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            var requisition = await _service.CreateAsync(
                request,
                cancellationToken);

            return CreatedAtAction(
                nameof(GetById),
                new { id = requisition.Id },
                requisition);
        }
        catch (ArgumentException exception)
        {
            return BadRequest(new { message = exception.Message });
        }
    }

    [HttpGet]
    [Authorize(Policy = "permission:requisitions.read")]
    [ProducesResponseType(typeof(IReadOnlyCollection<RequisitionResponse>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyCollection<RequisitionResponse>>> GetAll(
        CancellationToken cancellationToken)
    {
        var requisitions = await _service.GetAllAsync(cancellationToken);

        return Ok(requisitions);
    }

    [HttpGet("{id:guid}")]
    [Authorize(Policy = "permission:requisitions.read")]
    [ProducesResponseType(typeof(RequisitionResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<RequisitionResponse>> GetById(
        Guid id,
        CancellationToken cancellationToken)
    {
        var requisition = await _service.GetByIdAsync(
            id,
            cancellationToken);

        if (requisition is null)
            return NotFound();

        return Ok(requisition);
    }

    [HttpPost("{id:guid}/items")]
    [Authorize(Policy = "permission:requisitions.update")]
    [ProducesResponseType(typeof(RequisitionResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<RequisitionResponse>> AddItem(
        Guid id,
        [FromBody] AddRequisitionItemRequest request,
        CancellationToken cancellationToken)
        {
            try
            {
                var requisition = await _service.AddItemAsync(
                    id,
                    request,
                    cancellationToken);

                if (requisition is null)
                    return NotFound(new
                    {
                        message = "Requisition was not found."
                    });

                return Ok(requisition);
            }
            catch (ArgumentException exception)
            {
                return BadRequest(new
                {
                    message = exception.Message
                });
            }
            catch (InvalidOperationException exception)
            {
                return BadRequest(new
                {
                    message = exception.Message
                });
            }
    }

    [HttpPut("{id:guid}/items/{itemId:guid}")]
    [Authorize(Policy = "permission:requisitions.update")]
    [ProducesResponseType(typeof(RequisitionResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<RequisitionResponse>> UpdateItem(
        Guid id,
        Guid itemId,
        [FromBody] UpdateRequisitionItemRequest request,
        CancellationToken cancellationToken)
        {
            try
            {
                var requisition = await _service.UpdateItemAsync(
                    id,
                    itemId,
                    request,
                    cancellationToken);

                if (requisition is null)
                    return NotFound(new
                    {
                        message = "Requisition was not found."
                    });

                return Ok(requisition);
            }
            catch (ArgumentException exception)
            {
                return BadRequest(new
                {
                    message = exception.Message
                });
            }
            catch (InvalidOperationException exception)
            {
                return BadRequest(new
                {
                    message = exception.Message
                });
            }
            catch (KeyNotFoundException exception)
            {
                return NotFound(new
                {
                    message = exception.Message
                });
            }
    }

    [HttpDelete("{id:guid}/items/{itemId:guid}")]
    [Authorize(Policy = "permission:requisitions.update")]
    [ProducesResponseType(typeof(RequisitionResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<RequisitionResponse>> RemoveItem(
        Guid id,
        Guid itemId,
        CancellationToken cancellationToken)
        {
            try
            {
                var requisition = await _service.RemoveItemAsync(
                    id,
                    itemId,
                    cancellationToken);

                if (requisition is null)
                    return NotFound(new
                    {
                        message = "Requisition was not found."
                    });

                return Ok(requisition);
            }
            catch (InvalidOperationException exception)
            {
                return BadRequest(new
                {
                    message = exception.Message
                });
            }
    }


    [HttpPost("{id:guid}/cancel")]
    [Authorize(Policy = "permission:requisitions.cancel")]
    [ProducesResponseType(typeof(RequisitionResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<RequisitionResponse>> Cancel(
        Guid id,
        CancellationToken cancellationToken)
    {
        try
        {
            var requisition = await _service.CancelAsync(
                id,
                cancellationToken);

            if (requisition is null)
                return NotFound(new
                {
                    message = "Requisition was not found."
                });

            return Ok(requisition);
        }
        catch (InvalidOperationException exception)
        {
            return BadRequest(new
            {
                message = exception.Message
            });
        }
    }


    [HttpPost("{id:guid}/fulfill")]
    [Authorize(Policy = "permission:requisitions.fulfill")]
    [ProducesResponseType(typeof(RequisitionResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<RequisitionResponse>> Fulfill(
        Guid id,
        CancellationToken cancellationToken)
    {
        try
        {
            var requisition = await _service.FulfillAsync(
                id,
                cancellationToken);

            if (requisition is null)
                return NotFound(new
                {
                    message = "Requisition was not found."
                });

            return Ok(requisition);
        }
        catch (InvalidOperationException exception)
        {
            return BadRequest(new
            {
                message = exception.Message
            });
        }
    }
}