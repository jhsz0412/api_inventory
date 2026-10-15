using Microsoft.AspNetCore.Authorization;

namespace InventoryManagement.Api.Authorization;

public sealed class PermissionRequirement(string permissionCode)
    : IAuthorizationRequirement
{
    public string PermissionCode { get; } = permissionCode;
}