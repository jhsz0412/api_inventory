
using Microsoft.EntityFrameworkCore;
using Navigation.Application.Interfaces;
using Navigation.Domain.Entities;
using Navigation.Infrastructure.Persistence;

namespace Navigation.Infrastructure.Persistence.Repositories;

public sealed class MenuItemRepository(
    NavigationDbContext context) : IMenuItemRepository
{
    public async Task<MenuItem?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        return await context.MenuItems
            .FirstOrDefaultAsync(
                menu => menu.Id == id,
                cancellationToken);
    }

    public async Task<IReadOnlyList<MenuItem>> GetAllAsync(
        CancellationToken cancellationToken = default)
    {
        return await context.MenuItems
            .AsNoTracking()
            .OrderBy(menu => menu.SortOrder)
            .ThenBy(menu => menu.Name)
            .ToListAsync(cancellationToken);
    }

    public async Task<bool> ExistsByNameAsync(
        string name,
        Guid? excludingId = null,
        CancellationToken cancellationToken = default)
    {
        return await context.MenuItems.AnyAsync(
            menu => menu.Name == name &&
                    (!excludingId.HasValue ||
                     menu.Id != excludingId.Value),
            cancellationToken);
    }

    public async Task<bool> ParentExistsAsync(
        Guid parentId,
        CancellationToken cancellationToken = default)
    {
        return await context.MenuItems.AnyAsync(
            menu => menu.Id == parentId && menu.IsActive,
            cancellationToken);
    }

    public async Task AddAsync(
        MenuItem menuItem,
        CancellationToken cancellationToken = default)
    {
        await context.MenuItems.AddAsync(
            menuItem,
            cancellationToken);
    }

    public void Update(MenuItem menuItem)
    {
        context.MenuItems.Update(menuItem);
    }

    public async Task SaveChangesAsync(
        CancellationToken cancellationToken = default)
    {
        await context.SaveChangesAsync(cancellationToken);
    }
}
