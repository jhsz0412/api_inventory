
using Microsoft.AspNetCore.Mvc;
using Organization.Application.DTOs.Employees;
using Organization.Application.Services.Employees;

namespace Organization.Presentation.Controllers;

[ApiController]
[Route("api/employees")]
public sealed class EmployeesController(
    IEmployeeService employeeService) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAll(
        CancellationToken cancellationToken)
    {
        var employees = await employeeService.GetAllAsync(
            cancellationToken);

        return Ok(employees);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(
        Guid id,
        CancellationToken cancellationToken)
    {
        var employee = await employeeService.GetByIdAsync(
            id, cancellationToken);

        return employee is null ? NotFound() : Ok(employee);
    }

    [HttpPost]
    public async Task<IActionResult> Create(
        [FromBody] CreateEmployeeRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            var employee = await employeeService.CreateAsync(
                request, cancellationToken);

            return CreatedAtAction(
                nameof(GetById),
                new { id = employee.Id },
                employee);
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
        [FromBody] UpdateEmployeeRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            var employee = await employeeService.UpdateAsync(
                id, request, cancellationToken);

            return employee is null ? NotFound() : Ok(employee);
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
        [FromBody] UpdateEmployeeStatusRequest request,
        CancellationToken cancellationToken)
    {
        var updated = await employeeService.SetStatusAsync(
            id, request.IsActive, cancellationToken);

        return updated ? NoContent() : NotFound();
    }

    public sealed record UpdateEmployeeStatusRequest(bool IsActive);
}
