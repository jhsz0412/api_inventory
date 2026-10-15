using System.ComponentModel.DataAnnotations;

namespace Identity.Application.DTOs.Users;

public sealed class LoginRequest
{
    [Required]
    public string UserNameOrEmail { get; init; } = string.Empty;

    [Required]
    public string Password { get; init; } = string.Empty;
}
