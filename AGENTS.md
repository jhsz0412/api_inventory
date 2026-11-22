# Inventory Management System - AI Agent Instructions

## 1. Project Identity

This repository is the **Inventory Management System**.

It is a backend-focused enterprise application built using:

- C#
- ASP.NET Core Web API
- .NET 10
- Clean Architecture
- Modular Monolith Architecture
- Entity Framework Core
- SQL Server 2022
- JWT Authentication and Authorization

The solution is named `InventoryManagement.slnx`.

The local development API is:

`http://localhost:5067`

The primary database is `InventoryManagementDbV2`.

This is NOT an HRIS project.

## 2. Most Important Development Rules

Follow these rules at all times.

1. Understand the existing implementation before suggesting changes.
2. Inspect relevant source files before modifying anything.
3. Do not assume that a class, interface, service, or configuration exists. Verify it first.
4. Do not introduce unnecessary architectural changes.
5. Do not replace existing implementations without understanding their purpose.
6. Preserve existing naming conventions and coding patterns.
7. Do not generate unnecessary files, folders, abstractions, or dependencies.
8. Do not modify unrelated modules.
9. Explain architectural implications before implementing significant changes.
10. Never claim that a build, migration, test, or database operation succeeded without verifying its result.

### User approval requirement

The user prefers understanding the system before implementation.

Therefore:

- When asked to explain something, explain it without modifying files.
- When asked to analyze architecture, inspect and report findings without changing files.
- When asked to plan an implementation, provide the proposed changes first.
- Do not implement changes until the user explicitly authorizes implementation.
- Do not automatically run database migrations or destructive database operations.
- Never delete existing data, files, migrations, or database objects without explicit approval.

## 3. Architecture

The system follows a **Modular Monolith using Clean Architecture**.

The application is deployed as one backend application but organized into independent business modules.

Each module may contain four architectural projects:

### Domain

Responsible for:

- Business entities.
- Value objects.
- Domain rules.
- Domain exceptions.
- Business invariants.

Domain must not depend on Application, Infrastructure, Presentation, or external frameworks unnecessarily.

### Application

Responsible for:

- Use cases.
- Application services.
- DTOs.
- Interfaces.
- Business workflows.
- Application-level validation.

Application contains the abstractions required by its use cases.

### Infrastructure

Responsible for:

- Entity Framework Core.
- Database contexts.
- Entity configurations.
- Migrations.
- Repository implementations.
- External integrations.
- Persistence-related implementations.

Infrastructure implements abstractions defined by the inner layers.

### Presentation

Responsible for:

- ASP.NET Core API controllers.
- HTTP endpoints.
- Request and response handling.
- HTTP status codes.
- API-specific concerns.

Controllers should delegate application work rather than contain business logic.

### Dependency direction

Follow Clean Architecture dependency rules.

- Domain should remain independent.
- Application depends on Domain.
- Infrastructure implements Application abstractions.
- Presentation communicates with Application.
- Infrastructure and Presentation must not introduce unnecessary dependencies into Domain.

Always inspect existing project references before changing dependencies.

## 4. Solution Organization

The known repository structure includes:

`src/InventoryManagement.Api`

Main API host responsible for:

- Application startup.
- Dependency injection composition.
- Middleware registration.
- Authentication and authorization configuration.
- Module registration.
- API hosting.
- Cross-module integration.

`src/InventoryManagement.DatabaseSetup`

Responsible for database initialization and seed data.

`src/Modules`

Contains the business modules:

- Identity
- Organization
- Planning
- Requisitions
- Inventory
- Delivery
- Reporting
- Approvals
- Navigation

`src/BuildingBlocks`

Contains genuinely shared abstractions and components.

`tests`

Contains:

- ApiTests
- ArchitectureTests
- IntegrationTests
- UnitTests

Do not move responsibilities between these projects without explaining why.

## 5. Module Independence

Each module should own its business responsibilities.

Avoid creating unnecessary dependencies between modules.

For cross-module communication:

- Inspect existing integration mechanisms first.
- Prefer explicit interfaces and integration abstractions.
- Avoid directly accessing another module's internal repositories or DbContext.
- Avoid circular project references.
- Keep business logic within the module that owns it.

The API host is responsible for composing the modules.

## 6. Important Existing Business Implementations

The following are known project decisions. Verify their current implementation before changing them.

### Identity

Responsible for:

- Users.
- Roles.
- Permissions.
- User-role assignments.
- Authentication.
- JWT generation.

JWT configuration:

- Issuer: `InventoryManagement.Api`
- Audience: `InventoryManagement.Client`
- Access token lifetime: 15 minutes.

JWT claims have included employee ID, roles, and permissions.

Passwords must be securely hashed.

Never expose password hashes or secrets in API responses or logs.

### Organization

Responsible for:

- Employees.
- Departments.
- Groups.
- Group memberships.

Important group design decision:

The system uses **snapshot-based group membership**.

Group membership is represented through explicit `GroupMembers` records.

Groups are not restricted to one department.

An employee can belong to multiple groups.

Do not reintroduce the previous DepartmentId-based group design.

### Navigation

Responsible for metadata-driven menu items and group-based menu visibility.

Important decisions:

- Menu items are not directly permission-gated through a PermissionId column.
- Group-menu assignments are represented through `GroupMenuItems`.
- Group membership determines which menu items an employee can access.
- Parent and child menu relationships must be respected.
- Menu activation status is separate from group assignment.

Do not reintroduce the removed PermissionId relationship without explicit approval.

### Inventory

Responsible for:

- Items.
- Categories.
- Units of measurement.
- Warehouses.
- Stock-related functionality.

Respect existing soft-delete, restore, and permanent-delete behavior.

### Approvals

Responsible for approval workflows and approval steps.

Existing business rule:

An employee must not approve their own requisition when the workflow prohibits self-approval.

### Requisitions, Planning, Delivery and Reporting

Keep their business responsibilities within their respective modules.

Inspect their current implementations before proposing architectural changes.

## 7. Database and Persistence Rules

Database technology: SQL Server 2022 Developer Edition.

Use Entity Framework Core.

Preferred folder conventions:

- `Persistence/Configurations`
- `Persistence/Migrations`
- `Persistence/Repositories`

Rules:

1. Keep entity configurations separate from DbContext.
2. Use migrations to manage schema changes.
3. Never manually modify the database schema as a replacement for proper migrations.
4. Inspect the current database context before generating migrations.
5. Verify migration commands and their target projects.
6. Never apply migrations automatically without user authorization.
7. Never drop or recreate the production or development database without explicit permission.
8. Preserve existing data unless a migration explicitly requires an approved transformation.

Database credentials must remain in user-secrets or environment variables.

Never commit credentials or connection-string passwords.

## 8. API Development Conventions

Follow existing API conventions.

- Use controllers for HTTP handling.
- Use DTOs for API requests and responses.
- Use application services for use-case execution.
- Use dependency injection.
- Use appropriate HTTP status codes.
- Validate incoming requests.
- Handle exceptions consistently.
- Avoid exposing domain entities directly when existing DTO patterns are used.
- Follow existing route naming conventions.

Before creating an endpoint, inspect similar endpoints in the same module.

## 9. Authentication and Authorization

Authentication uses JWT.

Authorization includes role and permission-based policies where applicable.

Navigation visibility additionally uses group-based assignments.

Do not confuse:

- Authentication: Who is the user?
- Authorization: What operations can the user perform?
- Group membership: Which groups does the employee belong to?
- Menu visibility: Which navigation items are assigned to those groups?

These are separate concerns.

## 10. Coding Conventions

- Use modern C# features when consistent with the existing codebase.
- Follow existing namespace conventions.
- Use meaningful class, method, and variable names.
- Prefer constructor dependency injection.
- Keep classes focused on one responsibility.
- Avoid unnecessary abstractions.
- Avoid duplicated business logic.
- Use asynchronous database operations where appropriate.
- Pass CancellationToken where supported by existing conventions.
- Avoid synchronous blocking of asynchronous operations.
- Use nullable reference types consistently.
- Do not suppress compiler warnings without justification.

## 11. Windows Development Environment

The user works on Windows.

Use **Windows cmd.exe commands only**.

Never provide PowerShell commands.

Known project location:

`C:\Development\InventoryManagement`

Solution:

`InventoryManagement.slnx`

.NET SDK: 10.0.401

.NET runtime: 10.0.12

EF Core CLI: 10.0.12

SQL Server: 2022 Developer Edition.

### Common commands

Build:

`dotnet build InventoryManagement.slnx`

Run API:

`dotnet run --project src\InventoryManagement.Api\InventoryManagement.Api.csproj`

Run tests:

`dotnet test InventoryManagement.slnx`

Inspect SDK:

`dotnet --info`

Check EF CLI:

`dotnet ef --version`

Do not assume every command succeeds. Read and report the actual output.

## 12. Testing

The repository contains:

- ApiTests
- ArchitectureTests
- IntegrationTests
- UnitTests

When modifying code:

1. Identify the appropriate tests.
2. Check existing test conventions.
3. Add meaningful tests for changed behavior.
4. Run focused tests first when appropriate.
5. Run the full test suite when appropriate.
6. Report failures accurately.

Do not remove or weaken tests just to make a build pass.

## 13. Git and Repository Safety

- Inspect Git status before significant modifications.
- Preserve existing user changes.
- Never overwrite unrelated modifications.
- Do not automatically commit or push.
- Do not rewrite Git history without approval.
- Do not modify the remote repository without explicit authorization.

## 14. Required AI Working Process

For significant development tasks, follow this workflow:

### Phase 1: Understand

Inspect the relevant files and understand the current implementation.

### Phase 2: Explain

Explain:

- What currently exists.
- How it works.
- What problem needs solving.
- Which architectural layers are involved.

### Phase 3: Plan

Identify:

- Files to create.
- Files to modify.
- Files to remove, if any.
- Project dependencies.
- Database implications.
- API implications.
- Testing requirements.

### Phase 4: Obtain approval

Wait for the user's authorization before implementing significant changes.

### Phase 5: Implement

Make small, controlled changes.

Follow the existing architecture and conventions.

### Phase 6: Verify

Build, test, and inspect the results.

### Phase 7: Explain

Summarize:

- What changed.
- Why it changed.
- How the implementation works.
- Which files were affected.
- What the user should understand.
- Any remaining work.

## 15. Communication Style

The user wants to understand the entire backend system, not simply copy and paste code.

Therefore:

- Explain concepts in beginner-friendly language without sacrificing technical accuracy.
- Explain why a particular class, interface, folder, or project exists.
- Describe how components communicate.
- Use practical examples from the Inventory Management System.
- Explain unfamiliar architectural terminology.
- Do not skip important implementation details.
- Avoid overwhelming the user with unrelated information.
- Proceed step by step.
- Do not assume the user understands an implementation simply because the code compiles.

The main objective is to build a maintainable, secure, scalable, and well-understood Inventory Management System.
