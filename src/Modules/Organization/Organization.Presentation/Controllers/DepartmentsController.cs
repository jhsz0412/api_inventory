using Microsoft.AspNetCore.Mvc;
using Organization.Application.DTOs.Departments;
using Organization.Application.Services.Departments;
using Microsoft.AspNetCore.Authorization;

namespace Organization.Presentation.Controllers;

[ApiController]
[Route("api/departments")]
[Authorize(Policy = "permission:departments.read")]
public sealed class DepartmentsController(
    IDepartmentService departmentService) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAll(
        CancellationToken cancellationToken)
    {
        var departments = await departmentService.GetAllAsync(
            cancellationToken);

        return Ok(departments);
    }

    [HttpGet("{id:guid}")]
    [Authorize(Policy = "permission:departments.read")]
    public async Task<IActionResult> GetById(
        Guid id,
        CancellationToken cancellationToken)
    {
        var department = await departmentService.GetByIdAsync(
            id, cancellationToken);

        return department is null
            ? NotFound()
            : Ok(department);
    }

    [HttpPost]
    [Authorize(Policy = "permission:departments.create")]
    public async Task<IActionResult> Create(
        [FromBody] CreateDepartmentRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            var department = await departmentService.CreateAsync(
                request, cancellationToken);

            return CreatedAtAction(
                nameof(GetById),
                new { id = department.Id },
                department);
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
    [Authorize(Policy = "permission:departments.update")]
    public async Task<IActionResult> Update(
        Guid id,
        [FromBody] UpdateDepartmentRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            var department = await departmentService.UpdateAsync(
                id, request, cancellationToken);

            return department is null
                ? NotFound()
                : Ok(department);
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
    [Authorize(Policy = "permission:departments.update")]
    public async Task<IActionResult> UpdateStatus(
        Guid id,
        [FromBody] UpdateDepartmentStatusRequest request,
        CancellationToken cancellationToken)
    {
        var updated = await departmentService.SetStatusAsync(
            id, request.IsActive, cancellationToken);

        return updated ? NoContent() : NotFound();
    }

    public sealed record UpdateDepartmentStatusRequest(bool IsActive);
}