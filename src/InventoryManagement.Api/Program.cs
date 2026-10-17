using Delivery.Application.Interfaces;
using Delivery.Infrastructure;
using Delivery.Presentation.Controllers;
using Identity.Application.Interfaces;
using Identity.Infrastructure;
using Identity.Presentation.Controllers;
using Inventory.Infrastructure;
using Inventory.Presentation.Controllers;
using InventoryManagement.Api.Authorization;
using InventoryManagement.Api.Integrations;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;
using Organization.Infrastructure;
using Planning.Infrastructure;
using Planning.Presentation.Controllers;
using Reporting.Infrastructure;
using Reporting.Presentation.Controllers;
using Requisitions.Application.Interfaces;
using Requisitions.Infrastructure;
using Requisitions.Presentation.Controllers;
using System.Text;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Approvals.Infrastructure;
using Approvals.Presentation.Controllers;
using Navigation.Infrastructure;
using Navigation.Presentation.Controllers;


var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(options =>
{
    options.AddPolicy("Frontend", policy =>
    {
        var allowedOrigins = builder.Configuration
            .GetSection("Cors:AllowedOrigins")
            .Get<string[]>() ?? [];

        if (allowedOrigins.Length > 0)
        {
            policy
                .WithOrigins(allowedOrigins)
                .AllowAnyHeader()
                .AllowAnyMethod();
        }
    });
});


builder.Services
    .AddControllers()
    .AddApplicationPart(
        typeof(Organization.Presentation.Controllers.DepartmentsController).Assembly)
    .AddApplicationPart(
        typeof(FiscalYearsController).Assembly)
    .AddApplicationPart(
        typeof(RequisitionsController).Assembly)
    .AddApplicationPart(
        typeof(InventoryItemsController).Assembly)
    .AddApplicationPart(
        typeof(DeliveryController).Assembly)
    .AddApplicationPart(
        typeof(ReportingController).Assembly)
    .AddApplicationPart(
        typeof(UsersController).Assembly)
    .AddApplicationPart(
        typeof(RolesController).Assembly)
    .AddApplicationPart(
        typeof(ApprovalWorkflowsController).Assembly)
    .AddApplicationPart(
        typeof(MenuItemsController).Assembly);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Enter your JWT access token."
    });

    options.AddSecurityRequirement(document =>
        new OpenApiSecurityRequirement
        {
            [new OpenApiSecuritySchemeReference("Bearer", document)] = []
        });
});

builder.Services.AddIdentityInfrastructure(builder.Configuration);
builder.Services.AddOrganizationInfrastructure(builder.Configuration);
builder.Services.AddPlanningInfrastructure(builder.Configuration);
builder.Services.AddRequisitionsInfrastructure(builder.Configuration);
builder.Services.AddInventoryInfrastructure(builder.Configuration);
builder.Services.AddDeliveryInfrastructure(builder.Configuration);
builder.Services.AddReportingInfrastructure(builder.Configuration);
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddApprovalsInfrastructure(builder.Configuration);
builder.Services.AddNavigationInfrastructure(builder.Configuration);


var jwtSecretKey = builder.Configuration["Jwt:SecretKey"]
    ?? throw new InvalidOperationException(
        "JWT signing key is not configured.");

var jwtIssuer = builder.Configuration["Jwt:Issuer"]
    ?? throw new InvalidOperationException(
        "JWT issuer is not configured.");

var jwtAudience = builder.Configuration["Jwt:Audience"]
    ?? throw new InvalidOperationException(
        "JWT audience is not configured.");

if (Encoding.UTF8.GetByteCount(jwtSecretKey) < 32)
{
    throw new InvalidOperationException(
        "JWT signing key must contain at least 32 bytes.");
}

builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(jwtSecretKey)),

            ValidateIssuer = true,
            ValidIssuer = jwtIssuer,

            ValidateAudience = true,
            ValidAudience = jwtAudience,

            ValidateLifetime = true,
            ClockSkew = TimeSpan.FromSeconds(30)
        };


        options.Events = new JwtBearerEvents
        {
            OnTokenValidated = async context =>
            {
                var subject =
                    context.Principal?.FindFirst(JwtRegisteredClaimNames.Sub)?.Value
                    ?? context.Principal?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                if (!Guid.TryParse(subject, out var userId))
                {
                    context.Fail("Invalid token subject.");
                    return;
                }

                var userRepository = context.HttpContext.RequestServices
                    .GetRequiredService<IUserRepository>();

                var user = await userRepository.FindByIdAsync(
                    userId,
                    context.HttpContext.RequestAborted);


                if (user is null || !user.IsActive)
                {
                    context.Fail("The account is inactive or no longer exists.");
                    return;
                }

                var tokenId =
                    context.Principal?.FindFirst(JwtRegisteredClaimNames.Jti)?.Value
                    ?? context.Principal?.FindFirst("jti")?.Value;

                if (string.IsNullOrWhiteSpace(tokenId))
                {
                    context.Fail("Token ID is missing.");
                    return;
                }

                var revokedTokenRepository = context.HttpContext.RequestServices
                    .GetRequiredService<IRevokedTokenRepository>();

                var isRevoked = await revokedTokenRepository.IsRevokedAsync(
                    tokenId,
                    context.HttpContext.RequestAborted);

                if (isRevoked)
                {
                    context.Fail("The access token has been revoked.");
                }

            }
        };

    });

builder.Services.AddSingleton<
    IAuthorizationPolicyProvider,
    PermissionPolicyProvider>();

builder.Services.AddScoped<
    IAuthorizationHandler,
    PermissionAuthorizationHandler>();

builder.Services.AddAuthorization();

builder.Services.AddScoped<
    IRequisitionInventoryService,
    RequisitionInventoryService>();

builder.Services.AddScoped<
    IDeliveryDispatchWorkflow,
    DeliveryDispatchService>();

builder.Services.AddScoped<
    IEmployeeAccountLookup,
    EmployeeAccountLookup>();

builder.Services.AddScoped<
    RequisitionApprovalCoordinator>();

builder.Services.AddScoped<
    GroupMenuVisibilityService>();

builder.Services.AddScoped<
    Navigation.Application.Interfaces.IGroupMenuVisibilityService,
    GroupMenuVisibilityService>();

builder.Services.AddScoped<
    Navigation.Application.Interfaces.IGroupMenuAssignmentService,
    GroupMenuAssignmentService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}


app.UseHttpsRedirection();

app.UseCors("Frontend");

app.UseAuthentication();
app.UseAuthorization();


app.MapGet("/health", () => Results.Ok(new
{
    Status = "Healthy",
    Application = "InventoryManagement.Api",
    TimestampUtc = DateTime.UtcNow
}));

app.MapControllers();

app.Run();