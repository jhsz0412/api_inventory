\# Inventory Management System



A modular Inventory Management System built with \*\*ASP.NET Core, .NET 10, Clean Architecture, and a Modular Monolith architecture\*\*. The system is designed to organize inventory operations, requisitions, approvals, fulfillment, organizational structures, identity management, and reporting in a single application.



\## Overview



The Inventory Management System provides a structured foundation for managing inventory-related business processes through independently organized modules that share a common application host.



The project follows Clean Architecture principles to separate business rules, application logic, infrastructure concerns, and API presentation.



\## Architecture



The solution uses a \*\*Modular Monolith\*\* architecture with Clean Architecture principles.



\- \*\*Modular Monolith\*\* — groups related business capabilities into separate modules within one deployable application.

\- \*\*Clean Architecture\*\* — separates domain logic, application services, infrastructure, and presentation concerns.

\- \*\*Building Blocks\*\* — provides shared components and abstractions used across modules.

\- \*\*REST API\*\* — exposes application functionality through HTTP endpoints.

\- \*\*Entity Framework Core\*\* — supports database access and persistence.



The solution also contains shared `BuildingBlocks` projects and the `InventoryManagement.Api` application host.



\## Technology Stack



\- C#

\- .NET 10

\- ASP.NET Core Web API

\- Entity Framework Core

\- RESTful API design

\- JWT-based authentication

\- Role- and permission-based authorization

\- Git and GitHub



\## Project Structure



```text

InventoryManagement/

├── src/

│   ├── BuildingBlocks/

│   │   ├── BuildingBlocks.Application/

│   │   ├── BuildingBlocks.Domain/

│   │   └── BuildingBlocks.Infrastructure/

│   ├── InventoryManagement.Api/

│   └── Modules/

│       ├── Approvals/

│       ├── Delivery/

│       ├── Fulfillment/

│       ├── Identity/

│       ├── Inventory/

│       ├── Notifications/

│       ├── Organization/

│       ├── Planning/

│       ├── Reporting/

│       └── Requisitions/

├── tests/

│   ├── InventoryManagement.ApiTests/

│   ├── InventoryManagement.ArchitectureTests/

│   ├── InventoryManagement.IntegrationTests/

│   └── InventoryManagement.UnitTests/

├── .gitignore

└── InventoryManagement.slnx

```



Each business module may contain separate Domain, Application, Infrastructure, and Presentation projects according to its responsibilities.



\## Prerequisites



Before running the application, install:



\- \[.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0)

\- A database server compatible with the application's configured Entity Framework Core provider

\- Git



\## Getting Started



\### 1. Clone the repository



```bash

git clone https://github.com/jhsz0412/api\_inventory.git

cd api\_inventory

```



\### 2. Restore dependencies



```bash

dotnet restore InventoryManagement.slnx

```



\### 3. Configure the application



The application's local configuration files are excluded from version control to prevent accidental exposure of credentials.



Configure the required settings locally, including:



\- Database connection string

\- JWT secret key

\- Any additional environment-specific settings required by the application



Use the configuration keys expected by the application, including `Jwt:SecretKey` and the appropriate database connection-string key.



For local development, create an `appsettings.Development.json` file in the API project directory when needed. Never commit real passwords, connection strings containing credentials, or JWT signing secrets.



Use a sufficiently strong, randomly generated JWT signing secret and keep it outside source control.



\### 4. Apply database migrations



If the repository contains Entity Framework Core migrations, apply the appropriate migrations using the project's configured database context and migration setup.



The exact command depends on the context, migrations assembly, and database provider configured in the solution.



\### 5. Build the solution



```bash

dotnet build InventoryManagement.slnx

```



\### 6. Run the API



```bash

dotnet run --project src/InventoryManagement.Api/InventoryManagement.Api.csproj

```



Use the URL printed in the console to access the running API. If Swagger is enabled for the current environment, open its Swagger UI to explore the available endpoints.



\## Testing



The repository contains dedicated projects for:



\- API tests

\- Architecture tests

\- Integration tests

\- Unit tests



To execute the available tests:



```bash

dotnet test InventoryManagement.slnx

```



Test results depend on the current test implementation and any required database or environment configuration.



\## Security



\- Do not commit secrets, credentials, or production configuration files.

\- Keep local configuration files and environment-specific settings out of version control.

\- Use environment variables or a suitable secrets-management solution for sensitive settings.

\- Protect authenticated endpoints with appropriate authentication and authorization policies.

\- Use HTTPS in deployed environments.



\## Development Workflow



The `main` branch represents the primary development branch.



For larger changes, create a feature branch and submit a pull request before merging into `main`.



Example:



```bash

git checkout -b feature/your-feature-name

```



\## License



No license has been specified yet. Unless a license is added to the repository, others should not assume they have permission to redistribute or reuse the project's code.

