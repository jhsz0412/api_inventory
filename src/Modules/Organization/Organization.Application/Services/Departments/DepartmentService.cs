using Organization.Application.Contracts;
using Organization.Application.DTOs.Departments;

namespace Organization.Application.Services.Departments;

public sealed class DepartmentService(
    IDepartmentRepository repository) : IDepartmentService
{
    public async Task<IReadOnlyList<DepartmentResponse>> GetAllAsync(
        CancellationToken cancellationToken = default)
    {
        var departments = await repository.GetAllAsync(cancellationToken);

        return departments
            .Select(MapToResponse)
            .ToList();
    }

    public async Task<DepartmentResponse?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        var department = await repository.GetByIdAsync(id, cancellationToken);

        return department is null ? null : MapToResponse(department);
    }

    public async Task<DepartmentResponse> CreateAsync(
        CreateDepartmentRequest request,
        CancellationToken cancellationToken = default)
    {
        var code = NormalizeCode(request.Code);
        var name = NormalizeName(request.Name);

        Validate(code, name);

        if (await repository.ExistsByCodeAsync(
                code,
                cancellationToken: cancellationToken))
        {
            throw new InvalidOperationException(
                $"A department with code '{code}' already exists.");
        }

        var department = new Domain.Entities.Department(
            code,
            name);

        await repository.AddAsync(department, cancellationToken);
        await repository.SaveChangesAsync(cancellationToken);

        return MapToResponse(department);
    }

    public async Task<DepartmentResponse?> UpdateAsync(
        Guid id,
        UpdateDepartmentRequest request,
        CancellationToken cancellationToken = default)
    {
        var department = await repository.GetByIdAsync(id, cancellationToken);

        if (department is null)
            return null;

        var code = NormalizeCode(request.Code);
        var name = NormalizeName(request.Name);

        Validate(code, name);

        if (await repository.ExistsByCodeAsync(
                code,
                id,
                cancellationToken))
        {
            throw new InvalidOperationException(
                $"A department with code '{code}' already exists.");
        }

        department.UpdateDetails(code, name);

        await repository.SaveChangesAsync(cancellationToken);

        return MapToResponse(department);
    }

    public async Task<bool> SetStatusAsync(
        Guid id,
        bool isActive,
        CancellationToken cancellationToken = default)
    {
        var department = await repository.GetByIdAsync(id, cancellationToken);

        if (department is null)
            return false;

        if (isActive)
            department.Activate();
        else
            department.Deactivate();

        await repository.SaveChangesAsync(cancellationToken);

        return true;
    }

    private static string NormalizeCode(string? code) =>
        code?.Trim().ToUpperInvariant() ?? string.Empty;

    private static string NormalizeName(string? name) =>
        name?.Trim() ?? string.Empty;

    private static void Validate(string code, string name)
    {
        if (string.IsNullOrWhiteSpace(code))
            throw new ArgumentException("Department code is required.");

        if (code.Length > 30)
            throw new ArgumentException(
                "Department code cannot exceed 30 characters.");

        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Department name is required.");

        if (name.Length > 150)
            throw new ArgumentException(
                "Department name cannot exceed 150 characters.");
    }

    private static DepartmentResponse MapToResponse(
        Domain.Entities.Department department) =>
        new(
            department.Id,
            department.Code,
            department.Name,
            department.IsActive,
            department.CreatedAtUtc);
}