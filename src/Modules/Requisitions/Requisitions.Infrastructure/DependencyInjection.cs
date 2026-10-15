using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Requisitions.Application.Interfaces;
using Requisitions.Application.Services;
using Requisitions.Infrastructure.Persistence;
using Requisitions.Infrastructure.Persistence.Repositories;

namespace Requisitions.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddRequisitionsInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var connectionString = configuration
            .GetConnectionString("InventoryManagement")
            ?? throw new InvalidOperationException(
                "Connection string 'InventoryManagement' was not found.");

        services.AddDbContext<RequisitionsDbContext>(options =>
            options.UseSqlServer(connectionString));

        services.AddScoped<IRequisitionRepository, RequisitionRepository>();

        services.AddScoped<IRequisitionService, RequisitionService>();

        return services;
    }
}
