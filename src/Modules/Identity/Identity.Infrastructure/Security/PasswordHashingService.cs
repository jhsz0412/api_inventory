using Identity.Application.Interfaces;
using Microsoft.AspNetCore.Identity;

namespace Identity.Infrastructure.Security;

public sealed class PasswordHashingService
    : IPasswordHashingService
{
    private readonly PasswordHasher<string> _passwordHasher = new();

    public string HashPassword(string password)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(password);

        return _passwordHasher.HashPassword(
            "InventoryManagementUser",
            password);
    }

    public bool VerifyPassword(
        string passwordHash,
        string password)
    {
        if (string.IsNullOrWhiteSpace(passwordHash) ||
            string.IsNullOrEmpty(password))
        {
            return false;
        }

        var result = _passwordHasher.VerifyHashedPassword(
            "InventoryManagementUser",
            passwordHash,
            password);

        return result != PasswordVerificationResult.Failed;
    }
}