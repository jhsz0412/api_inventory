using Organization.Application.Contracts;
using Organization.Application.DTOs.Positions;
using Organization.Domain.Entities;

namespace Organization.Application.Services.Positions;

public sealed class PositionService(
    IPositionRepository positionRepository) : IPositionService
{
    public async Task<IReadOnlyList<PositionResponse>> GetAllAsync(
        CancellationToken cancellationToken = default)
    {
        var positions = await positionRepository.GetAllAsync(
            cancellationToken);

        return positions.Select(MapToResponse).ToList();
    }

    public async Task<PositionResponse?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        var position = await positionRepository.GetByIdAsync(
            id, cancellationToken);

        return position is null ? null : MapToResponse(position);
    }

    public async Task<PositionResponse> CreateAsync(
        CreatePositionRequest request,
        CancellationToken cancellationToken = default)
    {
        var code = NormalizeRequired(request.Code, "Position code");
        var name = NormalizeRequired(request.Name, "Position name");

        if (await positionRepository.ExistsByCodeAsync(
                code, cancellationToken: cancellationToken))
        {
            throw new InvalidOperationException(
                $"Position code '{code}' already exists.");
        }

        var position = new Position(code, name);

        await positionRepository.AddAsync(position, cancellationToken);
        await positionRepository.SaveChangesAsync(cancellationToken);

        return MapToResponse(position);
    }

    public async Task<PositionResponse?> UpdateAsync(
        Guid id,
        UpdatePositionRequest request,
        CancellationToken cancellationToken = default)
    {
        var position = await positionRepository.GetByIdAsync(
            id, cancellationToken);

        if (position is null)
            return null;

        var code = NormalizeRequired(request.Code, "Position code");
        var name = NormalizeRequired(request.Name, "Position name");

        if (await positionRepository.ExistsByCodeAsync(
                code, id, cancellationToken))
        {
            throw new InvalidOperationException(
                $"Position code '{code}' already exists.");
        }

        position.UpdateDetails(code, name);

        await positionRepository.SaveChangesAsync(cancellationToken);

        return MapToResponse(position);
    }

    public async Task<bool> SetStatusAsync(
        Guid id,
        bool isActive,
        CancellationToken cancellationToken = default)
    {
        var position = await positionRepository.GetByIdAsync(
            id, cancellationToken);

        if (position is null)
            return false;

        if (isActive)
            position.Activate();
        else
            position.Deactivate();

        await positionRepository.SaveChangesAsync(cancellationToken);

        return true;
    }

    private static string NormalizeRequired(
        string? value,
        string fieldName)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException($"{fieldName} is required.");

        return value.Trim();
    }

    private static PositionResponse MapToResponse(Position position)
    {
        return new PositionResponse(
            position.Id,
            position.Code,
            position.Name,
            position.IsActive,
            position.CreatedAtUtc);
    }
}
