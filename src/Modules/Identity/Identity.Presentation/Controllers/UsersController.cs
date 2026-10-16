using Identity.Application.DTOs.Users;
using Identity.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Identity.Presentation.Controllers;

[ApiController]
[Route("api/identity/users")]
public sealed class UsersController(
    IUserService userService,
    IRevokedTokenRepository revokedTokenRepository) : ControllerBase
{
    [Authorize(Policy = "permission:identity.users.manage")]
    [HttpPost]
    [ProducesResponseType(typeof(UserResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<UserResponse>> Create(
    [FromBody] CreateUserRequest request,
    CancellationToken cancellationToken)
    {
        try
        {
            var user = await userService.CreateAsync(request, cancellationToken);
            return StatusCode(StatusCodes.Status201Created, user);
        }
        catch (ArgumentException exception)
        {
            return BadRequest(new { message = exception.Message });
        }
        catch (InvalidOperationException exception)
        {
            return Conflict(new { message = exception.Message });
        }
    }

    [AllowAnonymous]
    [HttpPost("/api/identity/login")]
    [ProducesResponseType(typeof(LoginResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<LoginResponse>> Login(
    [FromBody] LoginRequest request,
    CancellationToken cancellationToken)
    {
        try
        {
            var response = await userService.LoginAsync(
                request,
                cancellationToken);

            return Ok(response);
        }
        catch (UnauthorizedAccessException)
        {
            return Unauthorized(new
            {
                message = "Invalid username/email or password."
            });
        }
    }

    [Authorize]
    [HttpGet("/api/identity/me")]
    public IActionResult GetCurrentUser()
    {
        return Ok(new
        {
            Id = User.FindFirstValue(ClaimTypes.NameIdentifier)
                 ?? User.FindFirstValue("sub"),
            EmployeeId = User.FindFirstValue("employeeId"),
            UserName = User.Identity?.Name,
            Email = User.FindFirstValue(ClaimTypes.Email)
        });
    }



    [Authorize]
    [HttpPost("/api/identity/logout")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Logout(
        CancellationToken cancellationToken)
    {
        var tokenId = User.FindFirstValue("jti");

        var authorizationHeader =
            Request.Headers.Authorization.ToString();

        const string bearerPrefix = "Bearer ";

        if (string.IsNullOrWhiteSpace(tokenId) ||
            !authorizationHeader.StartsWith(
                bearerPrefix,
                StringComparison.OrdinalIgnoreCase))
        {
            return BadRequest(new
            {
                message = "Unable to identify the current access token."
            });
        }

        var accessToken = authorizationHeader[bearerPrefix.Length..].Trim();

        if (string.IsNullOrWhiteSpace(accessToken))
        {
            return BadRequest(new
            {
                message = "The access token is missing."
            });
        }

        var expirationClaim = User.FindFirstValue("exp");

        if (!long.TryParse(expirationClaim, out var expirationUnixSeconds))
        {
            return BadRequest(new
            {
                message = "The access token expiration claim is missing."
            });
        }

        var expiresAtUtc =
            DateTimeOffset.FromUnixTimeSeconds(expirationUnixSeconds);

        await revokedTokenRepository.RevokeAsync(
            tokenId,
            expiresAtUtc,
            cancellationToken);

        return Ok(new
        {
            message = "Logged out successfully. The access token has been revoked."
        });
    }



}