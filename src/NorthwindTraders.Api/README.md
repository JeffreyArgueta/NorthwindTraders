NorthwindTraders.Api
--------------------

Purpose
- Minimal API project that exposes REST endpoints for Northwind resources (orders, customers, products, etc.).

Key files
- Program.cs — application startup and endpoint registrations.
- ValidatorEndpointFilter.cs / ValidationFilter.cs — endpoint-level validation helpers using FluentValidation.
- appsettings.json / appsettings.Development.json — configuration including DefaultConnection.
- NorthwindTraders.Api.http — example HTTP requests for manual testing.

Configuration & runtime
- Connection string: DefaultConnection (from appsettings or environment). Used by Infrastructure layer.
- ALLOWED_ORIGINS — comma-separated list of allowed CORS origins. The project registers a named "Default" CORS policy.
- ASPNETCORE_ENVIRONMENT controls which appsettings file is used (Development will use appsettings.Development.json).

How to run
- dotnet build src/NorthwindTraders.slnx
- dotnet run --project src/NorthwindTraders.Api
- or use dotnet watch in the Api project for hot reload.

Notes
- The API wires FluentValidation validators via explicit DI registrations and a small EndpointFilter to keep dependencies minimal.
- CORS policy is registered (named "Default") but not applied globally by default. Add app.UseCors("Default") in Program.cs to enable it globally.
- Be careful not to commit secrets (appsettings.Development.json currently contains an example credentials string). Use environment variables or a secret store for production.
