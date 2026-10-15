
using System.ComponentModel.DataAnnotations;

namespace Identity.Application.DTOs.UserRoles;

public sealed class AssignRoleToUserRequest
{
    [Required]
    public Guid RoleId { get; init; }
}

