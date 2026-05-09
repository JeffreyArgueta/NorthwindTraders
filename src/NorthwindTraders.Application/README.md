NorthwindTraders.Application
--------------------------

Purpose
- Contains application layer services, DTOs, and business logic. This layer orchestrates repository calls from the Infrastructure layer and exposes typed service interfaces used by the API.

Key files and folders
- DependencyInjection.cs — registers application services into DI.
- Services/ — contains service implementations (e.g., OrderService, CustomerService) that implement domain contract interfaces.
- Validation/ — FluentValidation validators for incoming DTOs.
- NorthwindTraders.Application.csproj — project file for build and package references.

Responsibilities
- Validate input using FluentValidation (validators are defined here and registered in the API project).
- Map domain entities to DTOs and implement business rules and coordination logic.
- Keep logic testable and independent from EF/DB concerns — repositories are injected via interfaces defined in Domain.Contracts.

How to work on it
- Add or update validators for DTOs under Validation/.
- Services should be small, unit-testable classes that do not directly depend on EF Core types.
