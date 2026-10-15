using Delivery.Application.Interfaces;
using Delivery.Application.Services;
using Delivery.Infrastructure.Persistence;
using Delivery.Infrastructure.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Delivery.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddDeliveryInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var connectionString =
            configuration.GetConnectionString("InventoryManagement")
            ?? throw new InvalidOperationException(
                "Connection string 'InventoryManagement' was not found.");

        services.AddDbContext<DeliveryDbContext>(options =>
            options.UseSqlServer(connectionString));

        services.AddScoped<IDeliveryRepository, DeliveryRepository>();

        services.AddScoped<IDeliveryService, DeliveryService>();

        return services;
    }
}