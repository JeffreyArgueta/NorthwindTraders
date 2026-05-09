using System.Threading;
using System.Threading.Tasks;
using Xunit;
using NorthwindTraders.Infrastructure.Repositories;
using NorthwindTraders.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using NorthwindTraders.Domain.Northwind.Entities;
using FluentAssertions;

namespace NorthwindTraders.Api.Tests;

public class CustomerRepositoryTests
{
    private NorthwindDbContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<NorthwindDbContext>().UseInMemoryDatabase("testdb").Options;
        var db = new NorthwindDbContext(options);
        db.Customers.Add(new Customer { CustomerID = "ALFKI", CompanyName = "Alfreds Futterkiste" });
        db.SaveChanges();
        return db;
    }

    [Fact]
    public async Task GetByIdAsync_ReturnsCustomer()
    {
        using var db = CreateContext();
        var repo = new CustomerRepository(db);
        var dto = await repo.GetByIdAsync("ALFKI", CancellationToken.None);
        dto.Should().NotBeNull();
        dto!.CompanyName.Should().Be("Alfreds Futterkiste");
    }
}
