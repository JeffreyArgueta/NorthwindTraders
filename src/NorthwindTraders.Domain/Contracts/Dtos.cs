namespace NorthwindTraders.Domain.Contracts
{
    // DTOs
    public record CustomerDto(string CustomerID, string CompanyName, string? ContactName = null, string? ContactTitle = null,
        string? Address = null, string? City = null, string? Region = null, string? PostalCode = null, string? Country = null,
        string? Phone = null, string? Fax = null);

    public record CreateCustomerDto(string CustomerID, string CompanyName, string? ContactName = null, string? ContactTitle = null,
        string? Address = null, string? City = null, string? Region = null, string? PostalCode = null, string? Country = null,
        string? Phone = null, string? Fax = null);

    public record UpdateCustomerDto(string? CompanyName = null, string? ContactName = null, string? ContactTitle = null,
        string? Address = null, string? City = null, string? Region = null, string? PostalCode = null, string? Country = null,
        string? Phone = null, string? Fax = null);

    public record EmployeeDto(int EmployeeID, string FirstName, string LastName, string? Title = null, string? TitleOfCourtesy = null,
        DateTime? BirthDate = null, DateTime? HireDate = null, string? Address = null, string? City = null, string? Region = null,
        string? PostalCode = null, string? Country = null, string? HomePhone = null, string? Extension = null, string? PhotoPath = null);

    public record CreateEmployeeDto(string LastName, string FirstName, string? Title = null, string? TitleOfCourtesy = null,
        DateTime? BirthDate = null, DateTime? HireDate = null, string? Address = null, string? City = null, string? Region = null,
        string? PostalCode = null, string? Country = null, string? HomePhone = null, string? Extension = null, int? ReportsTo = null, string? PhotoPath = null);

    public record UpdateEmployeeDto(string? LastName = null, string? FirstName = null, string? Title = null, string? TitleOfCourtesy = null,
        DateTime? BirthDate = null, DateTime? HireDate = null, string? Address = null, string? City = null, string? Region = null,
        string? PostalCode = null, string? Country = null, string? HomePhone = null, string? Extension = null, int? ReportsTo = null, string? PhotoPath = null);

    public record ShipperDto(int ShipperID, string CompanyName, string? Phone = null);

    public record CreateShipperDto(string CompanyName, string? Phone = null);

    public record UpdateShipperDto(string? CompanyName = null, string? Phone = null);

    public record OrderDetailDto(int OrderID, int ProductID, decimal UnitPrice, short Quantity, float Discount);

    public record CreateOrderDetailDto(int ProductID, decimal UnitPrice, short Quantity, float Discount);

    public record UpdateOrderDetailDto(decimal UnitPrice, short Quantity, float Discount);

    // Product / Supplier DTOs
    public record ProductDto(int ProductID, string ProductName, int? SupplierID, int? CategoryID,
        string? QuantityPerUnit, decimal UnitPrice, short UnitsInStock, short UnitsOnOrder, short ReorderLevel, bool Discontinued,
        int? SupplierIdForDto = null, int? CategoryIdForDto = null);

    public record CreateProductDto(string ProductName, int? SupplierID = null, int? CategoryID = null,
        string? QuantityPerUnit = null, decimal UnitPrice = 0m, short UnitsInStock = 0, short UnitsOnOrder = 0, short ReorderLevel = 0, bool Discontinued = false);

    public record UpdateProductDto(string? ProductName = null, int? SupplierID = null, int? CategoryID = null,
        string? QuantityPerUnit = null, decimal? UnitPrice = null, short? UnitsInStock = null, short? UnitsOnOrder = null, short? ReorderLevel = null, bool? Discontinued = null);

    public record SupplierDto(int SupplierID, string CompanyName, string? ContactName = null, string? ContactTitle = null,
        string? Address = null, string? City = null, string? Region = null, string? PostalCode = null, string? Country = null,
        string? Phone = null, string? Fax = null, string? HomePage = null);

    public record CreateSupplierDto(string CompanyName, string? ContactName = null, string? ContactTitle = null,
        string? Address = null, string? City = null, string? Region = null, string? PostalCode = null, string? Country = null,
        string? Phone = null, string? Fax = null, string? HomePage = null);

    public record UpdateSupplierDto(string? CompanyName = null, string? ContactName = null, string? ContactTitle = null,
        string? Address = null, string? City = null, string? Region = null, string? PostalCode = null, string? Country = null,
        string? Phone = null, string? Fax = null, string? HomePage = null);

    // Read-only lookup DTOs
    public record CategoryDto(int CategoryID, string CategoryName, string? Description = null);
    public record RegionDto(int RegionID, string RegionDescription);
    public record TerritoryDto(string TerritoryID, string TerritoryDescription, int RegionID);
    public record CustomerDemographicDto(string CustomerTypeID, string? CustomerDesc = null);
    public record EmployeeTerritoryDto(int EmployeeID, string TerritoryID);

    public record OrderDto(
        int OrderID,
        string? CustomerID,
        int? EmployeeID,
        DateTime? OrderDate,
        DateTime? RequiredDate,
        DateTime? ShippedDate,
        int? ShipVia,
        decimal Freight,
        string? ShipName,
        string? ShipAddress,
        string? ShipCity,
        string? ShipRegion,
        string? ShipPostalCode,
        string? ShipCountry,
        CustomerDto? Customer = null,
        EmployeeDto? Employee = null,
        ShipperDto? Shipper = null,
        IEnumerable<OrderDetailDto>? OrderDetails = null
    );

    public record CreateOrderDto(
        string? CustomerID,
        int? EmployeeID,
        DateTime? OrderDate,
        DateTime? RequiredDate,
        DateTime? ShippedDate,
        int? ShipVia,
        decimal Freight,
        string? ShipName,
        string? ShipAddress,
        string? ShipCity,
        string? ShipRegion,
        string? ShipPostalCode,
        string? ShipCountry,
        IEnumerable<CreateOrderDetailDto>? OrderDetails = null
    );

    public record UpdateOrderDto(
        string? CustomerID,
        int? EmployeeID,
        DateTime? OrderDate,
        DateTime? RequiredDate,
        DateTime? ShippedDate,
        int? ShipVia,
        decimal Freight,
        string? ShipName,
        string? ShipAddress,
        string? ShipCity,
        string? ShipRegion,
        string? ShipPostalCode,
        string? ShipCountry
    );
}
