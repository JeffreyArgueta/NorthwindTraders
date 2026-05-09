namespace NorthwindTraders.Domain.Northwind.Entities
{
    // Employees table
    public sealed class Employee
    {
        public int EmployeeID { get; set; }
        public string LastName { get; set; } = null!; // nvarchar(20)
        public string FirstName { get; set; } = null!; // nvarchar(10)
        public string? Title { get; set; } // nvarchar(30)
        public string? TitleOfCourtesy { get; set; } // nvarchar(25)
        public DateTime? BirthDate { get; set; } // datetime
        public DateTime? HireDate { get; set; } // datetime
        public string? Address { get; set; } // nvarchar(60)
        public string? City { get; set; } // nvarchar(15)
        public string? Region { get; set; } // nvarchar(15)
        public string? PostalCode { get; set; } // nvarchar(10)
        public string? Country { get; set; } // nvarchar(15)
        public string? HomePhone { get; set; } // nvarchar(24)
        public string? Extension { get; set; } // nvarchar(4)
        public byte[]? Photo { get; set; } // image
        public string? Notes { get; set; } // ntext
        public int? ReportsTo { get; set; } // FK -> Employees(EmployeeID)
        public string? PhotoPath { get; set; } // nvarchar(255)

        // Navigation
        public Employee? Manager { get; set; }
        public ICollection<Employee> Subordinates { get; set; } = new List<Employee>();
        public ICollection<Order> Orders { get; set; } = new List<Order>();
        public ICollection<EmployeeTerritory> EmployeeTerritories { get; set; } = new List<EmployeeTerritory>();
    }

    // Categories table
    public sealed class Category
    {
        public int CategoryID { get; set; }
        public string CategoryName { get; set; } = null!; // nvarchar(15)
        public string? Description { get; set; } // ntext
        public byte[]? Picture { get; set; } // image

        // Navigation
        public ICollection<Product> Products { get; set; } = new List<Product>();
    }

    // Customers table
    public sealed class Customer
    {
        public string CustomerID { get; set; } = null!; // nchar(5)
        public string CompanyName { get; set; } = null!; // nvarchar(40)
        public string? ContactName { get; set; } // nvarchar(30)
        public string? ContactTitle { get; set; } // nvarchar(30)
        public string? Address { get; set; } // nvarchar(60)
        public string? City { get; set; } // nvarchar(15)
        public string? Region { get; set; } // nvarchar(15)
        public string? PostalCode { get; set; } // nvarchar(10)
        public string? Country { get; set; } // nvarchar(15)
        public string? Phone { get; set; } // nvarchar(24)
        public string? Fax { get; set; } // nvarchar(24)

        // Navigation
        public ICollection<Order> Orders { get; set; } = new List<Order>();
        public ICollection<CustomerCustomerDemo> CustomerCustomerDemos { get; set; } = new List<CustomerCustomerDemo>();
    }

    // Shippers table
    public sealed class Shipper
    {
        public int ShipperID { get; set; }
        public string CompanyName { get; set; } = null!; // nvarchar(40)
        public string? Phone { get; set; } // nvarchar(24)

        // Navigation
        public ICollection<Order> Orders { get; set; } = new List<Order>();
    }

    // Suppliers table
    public sealed class Supplier
    {
        public int SupplierID { get; set; }
        public string CompanyName { get; set; } = null!; // nvarchar(40)
        public string? ContactName { get; set; } // nvarchar(30)
        public string? ContactTitle { get; set; } // nvarchar(30)
        public string? Address { get; set; } // nvarchar(60)
        public string? City { get; set; } // nvarchar(15)
        public string? Region { get; set; } // nvarchar(15)
        public string? PostalCode { get; set; } // nvarchar(10)
        public string? Country { get; set; } // nvarchar(15)
        public string? Phone { get; set; } // nvarchar(24)
        public string? Fax { get; set; } // nvarchar(24)
        public string? HomePage { get; set; } // ntext

        // Navigation
        public ICollection<Product> Products { get; set; } = new List<Product>();
    }

    // Orders table
    public sealed class Order
    {
        public int OrderID { get; set; }
        public string? CustomerID { get; set; } // nchar(5)
        public int? EmployeeID { get; set; }
        public DateTime? OrderDate { get; set; }
        public DateTime? RequiredDate { get; set; }
        public DateTime? ShippedDate { get; set; }
        public int? ShipVia { get; set; } // ShipperID
        public decimal Freight { get; set; } // money
        public string? ShipName { get; set; } // nvarchar(40)
        public string? ShipAddress { get; set; } // nvarchar(60)
        public string? ShipCity { get; set; } // nvarchar(15)
        public string? ShipRegion { get; set; } // nvarchar(15)
        public string? ShipPostalCode { get; set; } // nvarchar(10)
        public string? ShipCountry { get; set; } // nvarchar(15)

        // Navigation
        public Customer? Customer { get; set; }
        public Employee? Employee { get; set; }
        public Shipper? Shipper { get; set; }
        public ICollection<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();
    }

    // Products table
    public sealed class Product
    {
        public int ProductID { get; set; }
        public string ProductName { get; set; } = null!; // nvarchar(40)
        public int? SupplierID { get; set; }
        public int? CategoryID { get; set; }
        public string? QuantityPerUnit { get; set; } // nvarchar(20)
        public decimal UnitPrice { get; set; } // money
        public short UnitsInStock { get; set; }
        public short UnitsOnOrder { get; set; }
        public short ReorderLevel { get; set; }
        public bool Discontinued { get; set; }

        // Navigation
        public Supplier? Supplier { get; set; }
        public Category? Category { get; set; }
        public ICollection<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();
    }

    // Order Details table (composite PK: OrderID + ProductID)
    public sealed class OrderDetail
    {
        public int OrderID { get; set; }
        public int ProductID { get; set; }
        public decimal UnitPrice { get; set; } // money
        public short Quantity { get; set; }
        public float Discount { get; set; }

        // Navigation
        public Order? Order { get; set; }
        public Product? Product { get; set; }
    }

    // CustomerCustomerDemo table
    public sealed class CustomerCustomerDemo
    {
        public string CustomerID { get; set; } = null!; // nchar(5)
        public string CustomerTypeID { get; set; } = null!; // nchar(10)

        // Navigation
        public Customer? Customer { get; set; }
        public CustomerDemographic? CustomerDemographic { get; set; }
    }

    // CustomerDemographics table
    public sealed class CustomerDemographic
    {
        public string CustomerTypeID { get; set; } = null!; // nchar(10)
        public string? CustomerDesc { get; set; } // ntext

        // Navigation
        public ICollection<CustomerCustomerDemo> CustomerCustomerDemos { get; set; } = new List<CustomerCustomerDemo>();
    }

    // Region table
    public sealed class Region
    {
        public int RegionID { get; set; }
        public string RegionDescription { get; set; } = null!; // nchar(50)

        // Navigation
        public ICollection<Territory> Territories { get; set; } = new List<Territory>();
    }

    // Territories table
    public sealed class Territory
    {
        public string TerritoryID { get; set; } = null!; // nvarchar(20)
        public string TerritoryDescription { get; set; } = null!; // nchar(50)
        public int RegionID { get; set; }

        // Navigation
        public Region? Region { get; set; }
        public ICollection<EmployeeTerritory> EmployeeTerritories { get; set; } = new List<EmployeeTerritory>();
    }

    // EmployeeTerritories table
    public sealed class EmployeeTerritory
    {
        public int EmployeeID { get; set; }
        public string TerritoryID { get; set; } = null!;

        // Navigation
        public Employee? Employee { get; set; }
        public Territory? Territory { get; set; }
    }
}
