using Identity.Application.DTOs.Users;
using Identity.Application.Interfaces;
using Identity.Domain.Entities;

namespace Identity.Application.Services;

public sealed class UserService(
    IUserRepository userRepository,
    IEmployeeAccountLookup employeeAccountLookup,
    IPasswordHashingService passwordHashingService,
    IJwtTokenService jwtTokenService,
    IUserRoleRepository userRoleRepository,
    IRolePermissionRepository rolePermissionRepository)
    : IUserService
{
    public async Task<UserResponse> CreateAsync(
        CreateUserRequest request,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        if (request.EmployeeId == Guid.Empty)
            throw new ArgumentException("Employee is required.");

        if (string.IsNullOrWhiteSpace(request.UserName))
            throw new ArgumentException("Username is required.");

        if (string.IsNullOrWhiteSpace(request.Email))
            throw new ArgumentException("Email is required.");

        if (string.IsNullOrWhiteSpace(request.Password))
            throw new ArgumentException("Password is required.");

        var userName = request.UserName.Trim();
        var email = request.Email.Trim().ToLowerInvariant();

        if (!await employeeAccountLookup.IsActiveEmployeeAsync(
                request.EmployeeId, cancellationToken))
        {
            throw new InvalidOperationException(
                "The employee does not exist or is inactive.");
        }

        if (await userRepository.UserNameExistsAsync(
                userName, cancellationToken))
        {
            throw new InvalidOperationException(
                "The username is already in use.");
        }

        if (await userRepository.EmailExistsAsync(
                email, cancellationToken))
        {
            throw new InvalidOperationException(
                "The email is already in use.");
        }

        var user = new User
        {
            EmployeeId = request.EmployeeId,
            UserName = userName,
            Email = email,
            PasswordHash = passwordHashingService.HashPassword(
                request.Password),
            IsActive = true,
            CreatedUtc = DateTimeOffset.UtcNow
        };

        await userRepository.AddAsync(user, cancellationToken);
        await userRepository.SaveChangesAsync(cancellationToken);

        return new UserResponse
        {
            Id = user.Id,
            EmployeeId = user.EmployeeId,
            UserName = user.UserName,
            Email = user.Email,
            IsActive = user.IsActive,
            CreatedUtc = user.CreatedUtc
        };
    }

    public async Task<LoginResponse> LoginAsync(
    LoginRequest request,
    CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        if (string.IsNullOrWhiteSpace(request.UserNameOrEmail) ||
            string.IsNullOrEmpty(request.Password))
        {
            throw new UnauthorizedAccessException("Invalid username/email or password.");
        }

        var userNameOrEmail = request.UserNameOrEmail.Trim();

        var user = await userRepository.FindByUserNameOrEmailAsync(
            userNameOrEmail,
            cancellationToken);

        if (user is null ||
            !user.IsActive ||
            !passwordHashingService.VerifyPassword(
                user.PasswordHash,
                request.Password))
        {
            throw new UnauthorizedAccessException("Invalid username/email or password.");
        }

        var roles = await userRoleRepository.GetRolesForUserAsync(
        user.Id,
        cancellationToken);

        var roleNames = roles
            .Select(role => role.Name)
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToArray();

        var permissionCodes = new HashSet<string>(
            StringComparer.OrdinalIgnoreCase);

        foreach (var role in roles)
        {
            var permissions =
                await rolePermissionRepository.GetPermissionsForRoleAsync(
                    role.Id,
                    cancellationToken);

            foreach (var permission in permissions)
            {
                permissionCodes.Add(permission.Code);
            }
        }

        var accessToken = jwtTokenService.GenerateAccessToken(
            user,
            roleNames,
            permissionCodes.ToArray());

        return new LoginResponse
        {
            Id = user.Id,
            EmployeeId = user.EmployeeId,
            UserName = user.UserName,
            Email = user.Email,
            AccessToken = accessToken.AccessToken,
            TokenType = "Bearer",
            ExpiresAtUtc = accessToken.ExpiresAtUtc
        };
    }
}