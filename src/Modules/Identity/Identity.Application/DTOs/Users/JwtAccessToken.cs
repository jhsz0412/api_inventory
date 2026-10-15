namespace Identity.Application.DTOs.Users;

public sealed class JwtAccessToken
{
    public string AccessToken { get; init; } = string.Empty;
    public DateTimeOffset ExpiresAtUtc { get; init; }
}