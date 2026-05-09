NorthwindTraders.Domain
-----------------------

Purpose
- Contains domain contracts (interfaces and DTOs) and the Northwind entity definitions. This project defines the shape of data and service contracts shared across Application and Infrastructure layers.

Key files and folders
- Contracts/ — service interfaces (IOrderService, ICustomerService, repositories) and DTO definitions used by the Application and API layers.
- Northwind/ — entity classes that map one-to-one with the Northwind database tables (used by the Infrastructure layer's DbContext).
- NorthwindTraders.Domain.csproj — project file.

Responsibilities
- Keep domain types and contracts free of infrastructure-specific dependencies so they can be reused across layers and easily tested/mocked.

Notes
- DTOs are immutable record types where appropriate. Entities are POCOs matching the database schema and are used only by the Infrastructure layer.
