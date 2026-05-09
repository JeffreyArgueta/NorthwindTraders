using Microsoft.EntityFrameworkCore;
using NorthwindTraders.Domain.Northwind.Entities;

namespace NorthwindTraders.Infrastructure.Persistence
{
    public class NorthwindDbContext : DbContext
    {
        public NorthwindDbContext(DbContextOptions<NorthwindDbContext> options) : base(options)
        {
        }

        public DbSet<Employee> Employees { get; set; } = null!;
        public DbSet<Category> Categories { get; set; } = null!;
        public DbSet<Customer> Customers { get; set; } = null!;
        public DbSet<Shipper> Shippers { get; set; } = null!;
        public DbSet<Supplier> Suppliers { get; set; } = null!;
        public DbSet<Order> Orders { get; set; } = null!;
        public DbSet<Product> Products { get; set; } = null!;
        public DbSet<OrderDetail> OrderDetails { get; set; } = null!;
        public DbSet<CustomerCustomerDemo> CustomerCustomerDemos { get; set; } = null!;
        public DbSet<CustomerDemographic> CustomerDemographics { get; set; } = null!;
        public DbSet<Region> Regions { get; set; } = null!;
        public DbSet<Territory> Territories { get; set; } = null!;
        public DbSet<EmployeeTerritory> EmployeeTerritories { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Employee
            modelBuilder.Entity<Employee>(b =>
            {
                b.HasKey(e => e.EmployeeID);
                b.Property(e => e.LastName).HasMaxLength(20);
                b.Property(e => e.FirstName).HasMaxLength(10);
                b.HasOne(e => e.Manager).WithMany(m => m.Subordinates).HasForeignKey(e => e.ReportsTo).OnDelete(DeleteBehavior.Restrict);
            });

            // Category
            modelBuilder.Entity<Category>(b =>
            {
                b.HasKey(c => c.CategoryID);
                b.Property(c => c.CategoryName).HasMaxLength(15);
            });

            // Customer
            modelBuilder.Entity<Customer>(b =>
            {
                b.HasKey(c => c.CustomerID);
                b.Property(c => c.CustomerID).HasMaxLength(5).IsFixedLength();
                b.Property(c => c.CompanyName).HasMaxLength(40);
            });

            // Shipper
            modelBuilder.Entity<Shipper>(b =>
            {
                b.HasKey(s => s.ShipperID);
                b.Property(s => s.CompanyName).HasMaxLength(40);
            });

            // Supplier
            modelBuilder.Entity<Supplier>(b =>
            {
                b.HasKey(s => s.SupplierID);
                b.Property(s => s.CompanyName).HasMaxLength(40);
            });

            // Product
            modelBuilder.Entity<Product>(b =>
            {
                b.HasKey(p => p.ProductID);
                b.Property(p => p.ProductName).HasMaxLength(40);
                b.Property(p => p.UnitPrice).HasColumnType("money");
                b.HasOne(p => p.Supplier).WithMany(s => s.Products).HasForeignKey(p => p.SupplierID).OnDelete(DeleteBehavior.SetNull);
                b.HasOne(p => p.Category).WithMany(c => c.Products).HasForeignKey(p => p.CategoryID).OnDelete(DeleteBehavior.SetNull);
            });

            // Order
            modelBuilder.Entity<Order>(b =>
            {
                b.HasKey(o => o.OrderID);
                b.Property(o => o.Freight).HasColumnType("money").HasDefaultValue(0m);
                b.Property(o => o.CustomerID).HasMaxLength(5).IsFixedLength();
                b.HasOne(o => o.Customer).WithMany(c => c.Orders).HasForeignKey(o => o.CustomerID).OnDelete(DeleteBehavior.SetNull);
                b.HasOne(o => o.Employee).WithMany(e => e.Orders).HasForeignKey(o => o.EmployeeID).OnDelete(DeleteBehavior.SetNull);
                b.HasOne(o => o.Shipper).WithMany(s => s.Orders).HasForeignKey(o => o.ShipVia).OnDelete(DeleteBehavior.SetNull);
            });

            // OrderDetail (composite key)
            modelBuilder.Entity<OrderDetail>(b =>
            {
                // The original Northwind database table name contains a space: "Order Details".
                // Map the entity explicitly so EF queries the correct table name in the existing database.
                b.ToTable("Order Details");
                b.HasKey(od => new { od.OrderID, od.ProductID });
                b.Property(od => od.UnitPrice).HasColumnType("money");
                b.HasOne(od => od.Order).WithMany(o => o.OrderDetails).HasForeignKey(od => od.OrderID).OnDelete(DeleteBehavior.Cascade);
                b.HasOne(od => od.Product).WithMany(p => p.OrderDetails).HasForeignKey(od => od.ProductID).OnDelete(DeleteBehavior.Restrict);
            });

            // CustomerCustomerDemo (composite key)
            modelBuilder.Entity<CustomerCustomerDemo>(b =>
            {
                b.HasKey(ccd => new { ccd.CustomerID, ccd.CustomerTypeID });
                b.HasOne(ccd => ccd.Customer).WithMany(c => c.CustomerCustomerDemos).HasForeignKey(ccd => ccd.CustomerID).OnDelete(DeleteBehavior.Cascade);
                b.HasOne(ccd => ccd.CustomerDemographic).WithMany(cd => cd.CustomerCustomerDemos).HasForeignKey(ccd => ccd.CustomerTypeID).OnDelete(DeleteBehavior.Cascade);
            });

            // CustomerDemographic
            modelBuilder.Entity<CustomerDemographic>(b =>
            {
                b.HasKey(cd => cd.CustomerTypeID);
                b.Property(cd => cd.CustomerTypeID).HasMaxLength(10).IsFixedLength();
                // CustomerDesc is ntext in original schema; leaving default mapping
            });

            // Region
            modelBuilder.Entity<Region>(b =>
            {
                b.HasKey(r => r.RegionID);
                b.Property(r => r.RegionDescription).HasMaxLength(50).IsFixedLength();
            });

            // Territory
            modelBuilder.Entity<Territory>(b =>
            {
                b.HasKey(t => t.TerritoryID);
                b.Property(t => t.TerritoryID).HasMaxLength(20);
                b.Property(t => t.TerritoryDescription).HasMaxLength(50).IsFixedLength();
                b.HasOne(t => t.Region).WithMany(r => r.Territories).HasForeignKey(t => t.RegionID).OnDelete(DeleteBehavior.Cascade);
            });

            // EmployeeTerritory (composite key)
            modelBuilder.Entity<EmployeeTerritory>(b =>
            {
                b.HasKey(et => new { et.EmployeeID, et.TerritoryID });
                b.HasOne(et => et.Employee).WithMany(e => e.EmployeeTerritories).HasForeignKey(et => et.EmployeeID).OnDelete(DeleteBehavior.Cascade);
                b.HasOne(et => et.Territory).WithMany(t => t.EmployeeTerritories).HasForeignKey(et => et.TerritoryID).OnDelete(DeleteBehavior.Cascade);
            });
        }
    }
}
