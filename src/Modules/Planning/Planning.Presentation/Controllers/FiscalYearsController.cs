using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Planning.Application.Contracts;
using Planning.Application.DTOs.FiscalYears;

namespace Planning.Presentation.Controllers;

[ApiController]
[Route("api/planning/fiscal-years")]
public sealed class FiscalYearsController(
    IFiscalYearService fiscalYearService) : ControllerBase
{
    [HttpGet]
    [Authorize(Policy = "permission:planning.fiscal-years.read")]
    public async Task<ActionResult<IReadOnlyList<FiscalYearResponse>>> GetAll(
        CancellationToken cancellationToken)
    {
        var fiscalYears = await fiscalYearService.GetAllAsync(
            cancellationToken);

        return Ok(fiscalYears);
    }

    [HttpGet("{id:guid}")]
    [Authorize(Policy = "permission:planning.fiscal-years.read")]
    public async Task<ActionResult<FiscalYearResponse>> GetById(
        Guid id,
        CancellationToken cancellationToken)
    {
        var fiscalYear = await fiscalYearService.GetByIdAsync(
            id, cancellationToken);

        if (fiscalYear is null)
            return NotFound();

        return Ok(fiscalYear);
    }

    [HttpPost]
    [Authorize(Policy = "permission:planning.fiscal-years.create")]
    public async Task<ActionResult<FiscalYearResponse>> Create(
        CreateFiscalYearRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            var fiscalYear = await fiscalYearService.CreateAsync(
                request, cancellationToken);

            return CreatedAtAction(
                nameof(GetById),
                new { id = fiscalYear.Id },
                fiscalYear);
        }
        catch (ArgumentException exception)
        {
            return BadRequest(new { message = exception.Message });
        }
        catch (InvalidOperationException exception)
        {
            return Conflict(new { message = exception.Message });
        }
    }

    [HttpPut("{id:guid}")]
    [Authorize(Policy = "permission:planning.fiscal-years.update")]
    public async Task<ActionResult<FiscalYearResponse>> Update(
        Guid id,
        UpdateFiscalYearRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            var fiscalYear = await fiscalYearService.UpdateAsync(
                id, request, cancellationToken);

            if (fiscalYear is null)
                return NotFound();

            return Ok(fiscalYear);
        }
        catch (ArgumentException exception)
        {
            return BadRequest(new { message = exception.Message });
        }
        catch (InvalidOperationException exception)
        {
            return Conflict(new { message = exception.Message });
        }
    }

    [HttpPatch("{id:guid}/activate")]
    [Authorize(Policy = "permission:planning.fiscal-years.update")]
    public async Task<IActionResult> Activate(
        Guid id,
        CancellationToken cancellationToken)
    {
        var activated = await fiscalYearService.ActivateAsync(
            id, cancellationToken);

        return activated ? NoContent() : NotFound();
    }

    [HttpPatch("{id:guid}/deactivate")]
    [Authorize(Policy = "permission:planning.fiscal-years.update")]
    public async Task<IActionResult> Deactivate(
        Guid id,
        CancellationToken cancellationToken)
    {
        var deactivated = await fiscalYearService.DeactivateAsync(
            id, cancellationToken);

        return deactivated ? NoContent() : NotFound();
    }
}