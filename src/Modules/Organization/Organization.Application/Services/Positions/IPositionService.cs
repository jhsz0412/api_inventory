using Organization.Application.DTOs.Positions;

namespace Organization.Application.Services.Positions;

public interface IPositionService
{
    Task<IReadOnlyList<PositionResponse>> GetAllAsync(
        CancellationToken cancellationToken = default);

    Task<PositionResponse?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default);

    Task<PositionResponse> CreateAsync(
        CreatePositionRequest request,
        CancellationToken cancellationToken = default);

    Task<PositionResponse?> UpdateAsync(
        Guid id,
        UpdatePositionRequest request,
        CancellationToken cancellationToken = default);

    Task<bool> SetStatusAsync(
        Guid id,
        bool isActive,
        CancellationToken cancellationToken = default);
}
