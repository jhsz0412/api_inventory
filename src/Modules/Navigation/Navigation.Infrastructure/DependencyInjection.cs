
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Navigation.Application.Interfaces;
using Navigation.Application.Services;
using Navigation.Infrastructure.Persistence;
using Navigation.Infrastructure.Persistence.Repositories;

namespace Navigation.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddNavigationInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var connectionString = configuration
            .GetConnectionString("InventoryManagement")
            ?? throw new InvalidOperationException(
                "Connection string 'InventoryManagement' was not found.");

        services.AddDbContext<NavigationDbContext>(options =>
            options.UseSqlServer(connectionString));

        services.AddScoped<IMenuItemRepository, MenuItemRepository>();
        services.AddScoped<IMenuItemService, MenuItemService>();
        return services;
    }
}
