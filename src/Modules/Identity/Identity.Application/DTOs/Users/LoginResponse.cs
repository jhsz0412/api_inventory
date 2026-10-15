namespace Identity.Application.DTOs.Users;

public sealed class LoginResponse
{
    public Guid Id { get; init; }
    public Guid EmployeeId { get; init; }
    public string UserName { get; init; } = string.Empty;
    public string Email { get; init; } = string.Empty;
    public string AccessToken { get; init; } = string.Empty;
    public string TokenType { get; init; } = "Bearer";
    public DateTimeOffset ExpiresAtUtc { get; init; }
}