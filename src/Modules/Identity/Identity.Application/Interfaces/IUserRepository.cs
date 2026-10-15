using Identity.Domain.Entities;

namespace Identity.Application.Interfaces;

public interface IUserRepository
{
    Task<bool> UserNameExistsAsync(
        string userName,
        CancellationToken cancellationToken = default);

    Task<bool> EmailExistsAsync(
        string email,
        CancellationToken cancellationToken = default);

    Task AddAsync(
        User user,
        CancellationToken cancellationToken = default);

    Task SaveChangesAsync(
        CancellationToken cancellationToken = default);

    Task<User?> FindByUserNameOrEmailAsync(
        string userNameOrEmail,
        CancellationToken cancellationToken = default);

    Task<User?> FindByIdAsync(
        Guid userId,
        CancellationToken cancellationToken = default);

}