using System.ComponentModel.DataAnnotations;

namespace Identity.Application.DTOs.Users;

public sealed class CreateUserRequest
{
    [Required]
    public Guid EmployeeId { get; init; }

    [Required]
    [StringLength(100, MinimumLength = 3)]
    public string UserName { get; init; } = string.Empty;

    [Required]
    [EmailAddress]
    [StringLength(256)]
    public string Email { get; init; } = string.Empty;

    [Required]
    [StringLength(128, MinimumLength = 12)]
    public string Password { get; init; } = string.Empty;
}