using Microsoft.AspNetCore.Mvc;
using Organization.Application.DTOs.Positions;
using Organization.Application.Services.Positions;

namespace Organization.Presentation.Controllers;

[ApiController]
[Route("api/positions")]
public sealed class PositionsController(
    IPositionService positionService) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAll(
        CancellationToken cancellationToken)
    {
        var positions = await positionService.GetAllAsync(
            cancellationToken);

        return Ok(positions);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(
        Guid id,
        CancellationToken cancellationToken)
    {
        var position = await positionService.GetByIdAsync(
            id, cancellationToken);

        return position is null ? NotFound() : Ok(position);
    }

    [HttpPost]
    public async Task<IActionResult> Create(
        [FromBody] CreatePositionRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            var position = await positionService.CreateAsync(
                request, cancellationToken);

            return CreatedAtAction(
                nameof(GetById),
                new { id = position.Id },
                position);
        }
        catch (InvalidOperationException exception)
        {
            return Conflict(new { message = exception.Message });
        }
        catch (ArgumentException exception)
        {
            return BadRequest(new { message = exception.Message });
        }
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(
        Guid id,
        [FromBody] UpdatePositionRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            var position = await positionService.UpdateAsync(
                id, request, cancellationToken);

            return position is null ? NotFound() : Ok(position);
        }
        catch (InvalidOperationException exception)
        {
            return Conflict(new { message = exception.Message });
        }
        catch (ArgumentException exception)
        {
            return BadRequest(new { message = exception.Message });
        }
    }

    [HttpPatch("{id:guid}/status")]
    public async Task<IActionResult> UpdateStatus(
        Guid id,
        [FromBody] UpdatePositionStatusRequest request,
        CancellationToken cancellationToken)
    {
        var updated = await positionService.SetStatusAsync(
            id, request.IsActive, cancellationToken);

        return updated ? NoContent() : NotFound();
    }

    public sealed record UpdatePositionStatusRequest(bool IsActive);
}