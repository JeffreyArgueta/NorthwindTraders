NorthwindTraders.Infrastructure
------------------------------

Purpose
- Implements data access, EF Core DbContext, and repository classes. This layer maps domain entities to the existing Northwind database schema and provides repository implementations consumed by the Application layer.

Key files and folders
- Persistence/NorthwindDbContext.cs — EF Core DbContext, entity mappings and table configuration.
- Repositories/ — repository implementations (IOrderRepository, ICustomerRepository, etc.).
- DependencyInjection.cs — registers DbContext and repository services into DI.

Important notes
- Northwind uses some non-standard table names (for example, "Order Details" with a space). The DbContext maps those explicitly using ToTable(...) to match the physical schema.
- The DbContext configures composite keys and relationships required by the Northwind schema.
- Connection string comes from the API project's configuration and is used in AddNorthwindInfrastructure(...) during startup.

Database considerations
- For development, point DefaultConnection to a local SQL Server containing the Northwind schema (or run a containerized SQL Server with the schema loaded).
- Avoid committing production credentials into appsettings files; prefer environment variables or secrets.
