using Identity.Application.DTOs.Users;
using Identity.Domain.Entities;

namespace Identity.Application.Interfaces;

public interface IJwtTokenService
{
    JwtAccessToken GenerateAccessToken(
        User user,
        IReadOnlyCollection<string> roles,
        IReadOnlyCollection<string> permissions);
}