using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Organization.Application.Contracts;
using Organization.Application.Services.Departments;
using Organization.Application.Services.Employees;
using Organization.Application.Services.Groups;
using Organization.Application.Services.Positions;
using Organization.Infrastructure.Persistence;
using Organization.Infrastructure.Repositories;

namespace Organization.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddOrganizationInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var connectionString = configuration
            .GetConnectionString("InventoryManagement")
            ?? throw new InvalidOperationException(
                "Connection string 'InventoryManagement' was not found.");

        services.AddDbContext<OrganizationDbContext>(options =>
            options.UseSqlServer(connectionString));

        services.AddScoped<IDepartmentRepository, DepartmentRepository>();
        services.AddScoped<IDepartmentService, DepartmentService>();

        services.AddScoped<IGroupRepository, GroupRepository>();
        services.AddScoped<IGroupService, GroupService>();

        services.AddScoped<IPositionRepository, PositionRepository>();
        services.AddScoped<IPositionService, PositionService>();
        services.AddScoped<IEmployeeRepository, EmployeeRepository>();
        services.AddScoped<IEmployeeService, EmployeeService>();




        return services;
    }
}
