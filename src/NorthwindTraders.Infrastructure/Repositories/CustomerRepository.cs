using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using NorthwindTraders.Domain.Contracts;
using NorthwindTraders.Domain.Northwind.Entities;
using NorthwindTraders.Infrastructure.Persistence;

namespace NorthwindTraders.Infrastructure.Repositories
{
    public class CustomerRepository : BaseRepository, ICustomerRepository
    {
        public CustomerRepository(NorthwindDbContext db) : base(db) { }

        public async Task<PagedResult<CustomerDto>> GetAllAsync(int page = 1, int pageSize = 20, CancellationToken ct = default)
        {
            if (page < 1) page = 1;
            if (pageSize < 1) pageSize = 20;
            if (pageSize > 100) pageSize = 100;

            var query = _db.Customers
                .AsNoTracking()
                .OrderBy(c => c.CustomerID)
                .Select(c => new CustomerDto(c.CustomerID, c.CompanyName, c.ContactName, c.ContactTitle, c.Address, c.City, c.Region, c.PostalCode, c.Country, c.Phone, c.Fax));

            var total = await query.CountAsync(ct);
            var items = await query.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync(ct);
            return new PagedResult<CustomerDto>(items, total, page, pageSize);
        }

        public async Task<CustomerDto?> GetByIdAsync(string customerId, CancellationToken ct = default)
        {
            var c = await _db.Customers.AsNoTracking().FirstOrDefaultAsync(c => c.CustomerID == customerId, ct);
            if (c == null) return null;
            return new CustomerDto(c.CustomerID, c.CompanyName, c.ContactName, c.ContactTitle, c.Address, c.City, c.Region, c.PostalCode, c.Country, c.Phone, c.Fax);
        }

        public async Task<CustomerDto> CreateAsync(CreateCustomerDto dto, CancellationToken ct = default)
        {
            var c = new Customer { CustomerID = dto.CustomerID, CompanyName = dto.CompanyName, ContactName = dto.ContactName, ContactTitle = dto.ContactTitle, Address = dto.Address, City = dto.City, Region = dto.Region, PostalCode = dto.PostalCode, Country = dto.Country, Phone = dto.Phone, Fax = dto.Fax };
            _db.Customers.Add(c);
            await _db.SaveChangesAsync(ct);
            return new CustomerDto(c.CustomerID, c.CompanyName, c.ContactName, c.ContactTitle, c.Address, c.City, c.Region, c.PostalCode, c.Country, c.Phone, c.Fax);
        }

        public async Task<CustomerDto> UpdateAsync(string customerId, UpdateCustomerDto dto, CancellationToken ct = default)
        {
            var c = await _db.Customers.FirstOrDefaultAsync(x => x.CustomerID == customerId, ct);
            if (c == null) throw new KeyNotFoundException($"Customer {customerId} not found");
            c.CompanyName = dto.CompanyName ?? c.CompanyName;
            c.ContactName = dto.ContactName ?? c.ContactName;
            c.ContactTitle = dto.ContactTitle ?? c.ContactTitle;
            c.Address = dto.Address ?? c.Address;
            c.City = dto.City ?? c.City;
            c.Region = dto.Region ?? c.Region;
            c.PostalCode = dto.PostalCode ?? c.PostalCode;
            c.Country = dto.Country ?? c.Country;
            c.Phone = dto.Phone ?? c.Phone;
            c.Fax = dto.Fax ?? c.Fax;
            _db.Customers.Update(c);
            await _db.SaveChangesAsync(ct);
            return new CustomerDto(c.CustomerID, c.CompanyName, c.ContactName, c.ContactTitle, c.Address, c.City, c.Region, c.PostalCode, c.Country, c.Phone, c.Fax);
        }

        public async Task DeleteAsync(string customerId, CancellationToken ct = default)
        {
            var c = await _db.Customers.FindAsync(new object[] { customerId }, ct);
            if (c == null) throw new KeyNotFoundException($"Customer {customerId} not found");
            _db.Customers.Remove(c);
            await _db.SaveChangesAsync(ct);
        }
    }
}
