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
    public class SupplierRepository : BaseRepository, ISupplierRepository
    {
        public SupplierRepository(NorthwindDbContext db) : base(db) { }

        public async Task<PagedResult<SupplierDto>> GetAllAsync(int page = 1, int pageSize = 20, CancellationToken ct = default)
        {
            if (page < 1) page = 1;
            if (pageSize < 1) pageSize = 20;
            if (pageSize > 100) pageSize = 100;

            var query = _db.Suppliers
                .AsNoTracking()
                .OrderBy(s => s.SupplierID)
                .Select(s => new SupplierDto(s.SupplierID, s.CompanyName, s.ContactName, s.ContactTitle, s.Address, s.City, s.Region, s.PostalCode, s.Country, s.Phone, s.Fax, s.HomePage));

            var total = await query.CountAsync(ct);
            var items = await query.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync(ct);
            return new PagedResult<SupplierDto>(items, total, page, pageSize);
        }

        public async Task<SupplierDto?> GetByIdAsync(int supplierId, CancellationToken ct = default)
        {
            var s = await _db.Suppliers.AsNoTracking().FirstOrDefaultAsync(s => s.SupplierID == supplierId, ct);
            if (s == null) return null;
            return new SupplierDto(s.SupplierID, s.CompanyName, s.ContactName, s.ContactTitle, s.Address, s.City, s.Region, s.PostalCode, s.Country, s.Phone, s.Fax, s.HomePage);
        }

        public async Task<SupplierDto> CreateAsync(CreateSupplierDto dto, CancellationToken ct = default)
        {
            var s = new Supplier { CompanyName = dto.CompanyName, ContactName = dto.ContactName, ContactTitle = dto.ContactTitle, Address = dto.Address, City = dto.City, Region = dto.Region, PostalCode = dto.PostalCode, Country = dto.Country, Phone = dto.Phone, Fax = dto.Fax, HomePage = dto.HomePage };
            _db.Suppliers.Add(s);
            await _db.SaveChangesAsync(ct);
            return new SupplierDto(s.SupplierID, s.CompanyName, s.ContactName, s.ContactTitle, s.Address, s.City, s.Region, s.PostalCode, s.Country, s.Phone, s.Fax, s.HomePage);
        }

        public async Task<SupplierDto> UpdateAsync(int supplierId, UpdateSupplierDto dto, CancellationToken ct = default)
        {
            var s = await _db.Suppliers.FirstOrDefaultAsync(x => x.SupplierID == supplierId, ct);
            if (s == null) throw new KeyNotFoundException($"Supplier {supplierId} not found");
            s.CompanyName = dto.CompanyName ?? s.CompanyName;
            s.ContactName = dto.ContactName ?? s.ContactName;
            s.ContactTitle = dto.ContactTitle ?? s.ContactTitle;
            s.Address = dto.Address ?? s.Address;
            s.City = dto.City ?? s.City;
            s.Region = dto.Region ?? s.Region;
            s.PostalCode = dto.PostalCode ?? s.PostalCode;
            s.Country = dto.Country ?? s.Country;
            s.Phone = dto.Phone ?? s.Phone;
            s.Fax = dto.Fax ?? s.Fax;
            s.HomePage = dto.HomePage ?? s.HomePage;
            _db.Suppliers.Update(s);
            await _db.SaveChangesAsync(ct);
            return new SupplierDto(s.SupplierID, s.CompanyName, s.ContactName, s.ContactTitle, s.Address, s.City, s.Region, s.PostalCode, s.Country, s.Phone, s.Fax, s.HomePage);
        }

        public async Task DeleteAsync(int supplierId, CancellationToken ct = default)
        {
            var s = await _db.Suppliers.FindAsync(new object[] { supplierId }, ct);
            if (s == null) throw new KeyNotFoundException($"Supplier {supplierId} not found");
            _db.Suppliers.Remove(s);
            await _db.SaveChangesAsync(ct);
        }
    }
}
