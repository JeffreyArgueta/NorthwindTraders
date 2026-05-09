NorthwindTraders (src)
======================

Project overview
- This solution implements a small Northwind-style API split across four projects:
  - NorthwindTraders.Api — Minimal Web API exposing REST endpoints.
  - NorthwindTraders.Application — Application services and validators.
  - NorthwindTraders.Infrastructure — EF Core DbContext and repository implementations.
  - NorthwindTraders.Domain — Domain DTOs, entity classes, and service/repository contracts.

Main objective
- Provide a clean, layered reference implementation that exposes typical CRUD operations for Northwind entities (orders, customers, products, employees, etc.). The code is structured to be testable and maintainable with explicit DI wiring and minimal external dependencies.

How the layers interact
- Api -> Application -> Infrastructure -> Database
- The Api project depends on Application and Infrastructure for runtime composition and wiring. Application depends on Domain.Contracts and Infrastructure implements the repository contracts defined in Domain.Contracts.

Getting started
- Build solution: dotnet build NorthwindTraders.slnx
- Run API: dotnet run --project NorthwindTraders.Api
- Use appsettings.Development.json or environment variables to configure DefaultConnection and ALLOWED_ORIGINS.

Recommendations
- Keep secrets out of committed appsettings files; use environment variables or user secrets.
- Consider mapping all DB tables explicitly in the DbContext to avoid naming surprises with legacy schemas.
- Add integration tests that run against a disposable/test database (dockerized SQL Server or LocalDB) to catch mapping/configuration errors early.
