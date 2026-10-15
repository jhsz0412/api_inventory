
using System.ComponentModel.DataAnnotations;

namespace Identity.Application.DTOs.RolePermissions;

public sealed class AssignPermissionToRoleRequest
{
    [Required]
    public Guid PermissionId { get; init; }
}
