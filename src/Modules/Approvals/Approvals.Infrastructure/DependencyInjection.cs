
using Approvals.Application.Interfaces;
using Approvals.Application.Services;
using Approvals.Infrastructure.Persistence;
using Approvals.Infrastructure.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Approvals.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddApprovalsInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var connectionString = configuration
            .GetConnectionString("InventoryManagement")
            ?? throw new InvalidOperationException(
                "Connection string 'InventoryManagement' was not found.");

        services.AddDbContext<ApprovalsDbContext>(options =>
            options.UseSqlServer(connectionString));

        services.AddScoped<IApprovalsRepository, ApprovalsRepository>();
        services.AddScoped<IApprovalWorkflowService, ApprovalWorkflowService>();
        services.AddScoped<IRequisitionApprovalService,RequisitionApprovalService>();
        services.AddScoped<IApprovalAuditService, ApprovalAuditService>();

        return services;
    }
}
