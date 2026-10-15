using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Identity.Application.DTOs.Users;
using Identity.Application.Interfaces;
using Identity.Domain.Entities;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace Identity.Infrastructure.Security;

public sealed class JwtTokenService(
    IConfiguration configuration) : IJwtTokenService
{
    public JwtAccessToken GenerateAccessToken(
        User user,
        IReadOnlyCollection<string> roles,
        IReadOnlyCollection<string> permissions)
    {
        ArgumentNullException.ThrowIfNull(user);
        ArgumentNullException.ThrowIfNull(roles);
        ArgumentNullException.ThrowIfNull(permissions);

        var secretKey = configuration["Jwt:SecretKey"]
            ?? throw new InvalidOperationException(
                "JWT signing key is not configured.");

        if (Encoding.UTF8.GetByteCount(secretKey) < 32)
        {
            throw new InvalidOperationException(
                "JWT signing key must contain at least 32 bytes.");
        }

        var issuer = configuration["Jwt:Issuer"]
            ?? throw new InvalidOperationException(
                "JWT issuer is not configured.");

        var audience = configuration["Jwt:Audience"]
            ?? throw new InvalidOperationException(
                "JWT audience is not configured.");

        //var tokenMinutes = 15;

        //if (int.TryParse(
        //        configuration["Jwt:AccessTokenMinutes"],
        //        out var configuredMinutes))
        //{
        //    tokenMinutes = configuredMinutes;
        //}

        //if (tokenMinutes <= 0)
        //{
        //    throw new InvalidOperationException(
        //        "JWT expiration must be greater than zero.");
        //}

        //This keeps the default at 15 minutes
        //and rejects invalid values or lifetimes greater than 60 minutes.
        //The 60 - minute limit is a security policy choice; 
        //adjust it if requirements call for a different maximum.


        const int defaultTokenMinutes = 15;
        const int maximumTokenMinutes = 60;

        var configuredTokenMinutes =
            configuration["Jwt:AccessTokenMinutes"];

        var tokenMinutes = defaultTokenMinutes;

        if (!string.IsNullOrWhiteSpace(configuredTokenMinutes))
        {
            if (!int.TryParse(configuredTokenMinutes, out tokenMinutes))
            {
                throw new InvalidOperationException(
                    "JWT access token lifetime must be a valid integer.");
            }
        }

        if (tokenMinutes <= 0 || tokenMinutes > maximumTokenMinutes)
        {
            throw new InvalidOperationException(
                $"JWT access token lifetime must be between 1 and {maximumTokenMinutes} minutes.");
        }


        var now = DateTimeOffset.UtcNow;
        var expiresAtUtc = now.AddMinutes(tokenMinutes);

        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new("employeeId", user.EmployeeId.ToString()),
            new(ClaimTypes.Name, user.UserName),
            new(ClaimTypes.Email, user.Email)
        };

        claims.AddRange(
            roles.Where(role => !string.IsNullOrWhiteSpace(role))
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .Select(role => new Claim(ClaimTypes.Role, role)));

        claims.AddRange(
            permissions.Where(permission =>
                    !string.IsNullOrWhiteSpace(permission))
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .Select(permission => new Claim("permission", permission)));

        var key = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(secretKey));

        var credentials = new SigningCredentials(
            key,
            SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: issuer,
            audience: audience,
            claims: claims,
            notBefore: now.UtcDateTime,
            expires: expiresAtUtc.UtcDateTime,
            signingCredentials: credentials);

        return new JwtAccessToken
        {
            AccessToken = new JwtSecurityTokenHandler().WriteToken(token),
            ExpiresAtUtc = expiresAtUtc
        };
    }
}