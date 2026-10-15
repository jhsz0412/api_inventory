namespace Identity.Application.DTOs.Users;

public sealed class UserResponse
{
    public Guid Id { get; init; }

    public Guid EmployeeId { get; init; }

    public string UserName { get; init; } = string.Empty;

    public string Email { get; init; } = string.Empty;

    public bool IsActive { get; init; }

    public DateTimeOffset CreatedUtc { get; init; }
}