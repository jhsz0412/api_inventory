
using Approvals.Infrastructure;
using Approvals.Infrastructure.Persistence;
using Delivery.Infrastructure;
using Delivery.Infrastructure.Persistence;
using Identity.Infrastructure;
using Identity.Infrastructure.Persistence;
using Inventory.Application.Services;
using Inventory.Infrastructure;
using Inventory.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Organization.Infrastructure;
using Organization.Infrastructure.Persistence;
using Planning.Infrastructure;
using Planning.Infrastructure.Persistence;
using Requisitions.Infrastructure;
using Requisitions.Infrastructure.Persistence;
using Navigation.Infrastructure;
using Navigation.Infrastructure.Persistence;

var apiProjectPath = Path.GetFullPath(
    Path.Combine(
        AppContext.BaseDirectory,
        "..", "..", "..", "..",
        "InventoryManagement.Api"));

var configuration = new ConfigurationBuilder()
    .SetBasePath(apiProjectPath)
    .AddJsonFile("appsettings.json", optional: false)
    .AddJsonFile("appsettings.Development.json", optional: true)
    .AddEnvironmentVariables()
    .Build();

var connectionString =
    configuration.GetConnectionString("InventoryManagement");

if (string.IsNullOrWhiteSpace(connectionString))
{
    throw new InvalidOperationException(
        "Connection string 'InventoryManagement' was not found.");
}

var services = new ServiceCollection();

services.AddLogging(logging =>
    logging.AddConsole());

services.AddIdentityInfrastructure(configuration);
services.AddOrganizationInfrastructure(configuration);
services.AddPlanningInfrastructure(configuration);
services.AddRequisitionsInfrastructure(configuration);
services.AddInventoryInfrastructure(configuration);
services.AddDeliveryInfrastructure(configuration);
services.AddApprovalsInfrastructure(configuration);
services.AddNavigationInfrastructure(configuration);

await using var serviceProvider = services.BuildServiceProvider();

await using var scope = serviceProvider.CreateAsyncScope();

var contexts = new DbContext[]
{
    scope.ServiceProvider.GetRequiredService<IdentityDbContext>(),
    scope.ServiceProvider.GetRequiredService<OrganizationDbContext>(),
    scope.ServiceProvider.GetRequiredService<PlanningDbContext>(),
    scope.ServiceProvider.GetRequiredService<RequisitionsDbContext>(),
    scope.ServiceProvider.GetRequiredService<InventoryDbContext>(),
    scope.ServiceProvider.GetRequiredService<DeliveryDbContext>(),
    scope.ServiceProvider.GetRequiredService<ApprovalsDbContext>(),
    scope.ServiceProvider.GetRequiredService<NavigationDbContext>()
};

Console.WriteLine("Inventory Management database migration");
Console.WriteLine($"Configuration directory: {apiProjectPath}");
Console.WriteLine("Target database: InventoryManagementDbV2");
Console.WriteLine();

foreach (var context in contexts)
{
    Console.WriteLine(
        $"Applying migrations: {context.GetType().Name}");

    await context.Database.MigrateAsync();

    Console.WriteLine("Completed.");
}

Console.WriteLine();
Console.WriteLine("All database migrations completed.");

// Seed default organization data
Console.WriteLine();
Console.WriteLine("Seeding default organization data...");

var organizationDb = scope.ServiceProvider
    .GetRequiredService<OrganizationDbContext>();

await InventoryManagement.DatabaseSetup.Seeding.OrganizationSeeder
    .SeedAsync(organizationDb);

// Seed default Identity data
Console.WriteLine("Default organization data seeded.");

var identityDb = scope.ServiceProvider
    .GetRequiredService<IdentityDbContext>();

await InventoryManagement.DatabaseSetup.Seeding.IdentitySeeder
    .SeedAsync(
        identityDb,
        organizationDb);

Console.WriteLine("Default Identity data seeded.");

// Seed default Planning data
Console.WriteLine();
Console.WriteLine("Seeding default Planning data...");

var planningDb = scope.ServiceProvider
    .GetRequiredService<PlanningDbContext>();

await InventoryManagement.DatabaseSetup.Seeding.FiscalYearSeeder
    .SeedAsync(planningDb);

Console.WriteLine("Default Planning data seeded.");

// Seed default Inventory data
Console.WriteLine();
Console.WriteLine("Seeding default Inventory data...");

var inventoryDb = scope.ServiceProvider
    .GetRequiredService<InventoryDbContext>();

await InventoryManagement.DatabaseSetup.Seeding.InventorySeeder
    .SeedAsync(inventoryDb);

Console.WriteLine("Default Inventory data seeded.");

// Seed opening stock
Console.WriteLine();
Console.WriteLine("Seeding default opening stock...");

var inventoryService = scope.ServiceProvider
    .GetRequiredService<IInventoryService>();

await InventoryManagement.DatabaseSetup.Seeding.OpeningStockSeeder
    .SeedAsync(
        inventoryDb,
        inventoryService);

Console.WriteLine("Default opening stock seeding completed.");

//Approvals seeding
// Seed the default department approval workflow
Console.WriteLine();
Console.WriteLine("Seeding default approval workflow...");

var approvalsDb = scope.ServiceProvider
    .GetRequiredService<ApprovalsDbContext>();

await InventoryManagement.DatabaseSetup.Seeding.ApprovalWorkflowSeeder
    .SeedAsync(
        approvalsDb,
        organizationDb);

Console.WriteLine("Default approval workflow seeding completed.");

// Seed default Navigation data
Console.WriteLine();
Console.WriteLine("Seeding default Navigation data...");

var navigationDb = scope.ServiceProvider
    .GetRequiredService<NavigationDbContext>();

await InventoryManagement.DatabaseSetup.Seeding.NavigationSeeder
    .SeedAsync(
        navigationDb,
        identityDb);

Console.WriteLine("Default Navigation data seeded.");

