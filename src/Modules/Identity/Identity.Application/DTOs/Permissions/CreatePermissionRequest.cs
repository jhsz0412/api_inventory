
using System.ComponentModel.DataAnnotations;

namespace Identity.Application.DTOs.Permissions;

public sealed class CreatePermissionRequest
{
    [Required]
    [StringLength(150, MinimumLength = 3)]
    public string Code { get; init; } = string.Empty;

    [Required]
    [StringLength(150, MinimumLength = 2)]
    public string Name { get; init; } = string.Empty;

    [StringLength(500)]
    public string? Description { get; init; }
}
