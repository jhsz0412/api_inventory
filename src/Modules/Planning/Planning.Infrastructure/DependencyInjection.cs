using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Planning.Application.Contracts;
using Planning.Application.Services.FiscalYears;
using Planning.Infrastructure.Persistence;
using Planning.Infrastructure.Repositories;

namespace Planning.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddPlanningInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var connectionString = configuration
            .GetConnectionString("InventoryManagement")
            ?? throw new InvalidOperationException(
                "Connection string 'InventoryManagement' was not found.");

        services.AddDbContext<PlanningDbContext>(options =>
            options.UseSqlServer(connectionString));

        services.AddScoped<IFiscalYearRepository, FiscalYearRepository>();
        services.AddScoped<IFiscalYearService, FiscalYearService>();

        return services;
    }
}