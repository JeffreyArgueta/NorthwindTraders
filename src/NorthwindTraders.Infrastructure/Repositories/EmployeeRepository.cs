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
    public class EmployeeRepository : BaseRepository, IEmployeeRepository
    {
        public EmployeeRepository(NorthwindDbContext db) : base(db) { }

        public async Task<PagedResult<EmployeeDto>> GetAllAsync(int page = 1, int pageSize = 20, CancellationToken ct = default)
        {
            if (page < 1) page = 1;
            if (pageSize < 1) pageSize = 20;

            var query = _db.Employees
                .AsNoTracking()
                .OrderBy(e => e.EmployeeID)
                .Select(e => new EmployeeDto(e.EmployeeID, e.FirstName, e.LastName, e.Title, e.TitleOfCourtesy, e.BirthDate, e.HireDate, e.Address, e.City, e.Region, e.PostalCode, e.Country, e.HomePhone, e.Extension, e.PhotoPath));
            var total = await query.CountAsync(ct);
            var items = await query.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync(ct);
            return new PagedResult<EmployeeDto>(items, total, page, pageSize);
        }

        public async Task<EmployeeDto?> GetByIdAsync(int employeeId, CancellationToken ct = default)
        {
            var e = await _db.Employees.FirstOrDefaultAsync(e => e.EmployeeID == employeeId, ct);
            if (e == null) return null;
            return new EmployeeDto(e.EmployeeID, e.FirstName, e.LastName, e.Title, e.TitleOfCourtesy, e.BirthDate, e.HireDate, e.Address, e.City, e.Region, e.PostalCode, e.Country, e.HomePhone, e.Extension, e.PhotoPath);
        }

        public async Task<EmployeeDto> CreateAsync(CreateEmployeeDto dto, CancellationToken ct = default)
        {
            var e = new Employee { FirstName = dto.FirstName, LastName = dto.LastName, Title = dto.Title, TitleOfCourtesy = dto.TitleOfCourtesy, BirthDate = dto.BirthDate, HireDate = dto.HireDate, Address = dto.Address, City = dto.City, Region = dto.Region, PostalCode = dto.PostalCode, Country = dto.Country, HomePhone = dto.HomePhone, Extension = dto.Extension, ReportsTo = dto.ReportsTo, PhotoPath = dto.PhotoPath };
            _db.Employees.Add(e);
            await _db.SaveChangesAsync(ct);
            return new EmployeeDto(e.EmployeeID, e.FirstName, e.LastName, e.Title, e.TitleOfCourtesy, e.BirthDate, e.HireDate, e.Address, e.City, e.Region, e.PostalCode, e.Country, e.HomePhone, e.Extension, e.PhotoPath);
        }

        public async Task<EmployeeDto> UpdateAsync(int employeeId, UpdateEmployeeDto dto, CancellationToken ct = default)
        {
            var e = await _db.Employees.FirstOrDefaultAsync(x => x.EmployeeID == employeeId, ct);
            if (e == null) throw new KeyNotFoundException($"Employee {employeeId} not found");
            e.LastName = dto.LastName ?? e.LastName;
            e.FirstName = dto.FirstName ?? e.FirstName;
            e.Title = dto.Title ?? e.Title;
            e.TitleOfCourtesy = dto.TitleOfCourtesy ?? e.TitleOfCourtesy;
            e.BirthDate = dto.BirthDate ?? e.BirthDate;
            e.HireDate = dto.HireDate ?? e.HireDate;
            e.Address = dto.Address ?? e.Address;
            e.City = dto.City ?? e.City;
            e.Region = dto.Region ?? e.Region;
            e.PostalCode = dto.PostalCode ?? e.PostalCode;
            e.Country = dto.Country ?? e.Country;
            e.HomePhone = dto.HomePhone ?? e.HomePhone;
            e.Extension = dto.Extension ?? e.Extension;
            e.ReportsTo = dto.ReportsTo ?? e.ReportsTo;
            e.PhotoPath = dto.PhotoPath ?? e.PhotoPath;
            _db.Employees.Update(e);
            await _db.SaveChangesAsync(ct);
            return new EmployeeDto(e.EmployeeID, e.FirstName, e.LastName, e.Title, e.TitleOfCourtesy, e.BirthDate, e.HireDate, e.Address, e.City, e.Region, e.PostalCode, e.Country, e.HomePhone, e.Extension, e.PhotoPath);
        }

        public async Task DeleteAsync(int employeeId, CancellationToken ct = default)
        {
            var e = await _db.Employees.FindAsync(new object[] { employeeId }, ct);
            if (e == null) throw new KeyNotFoundException($"Employee {employeeId} not found");
            _db.Employees.Remove(e);
            await _db.SaveChangesAsync(ct);
        }
    }
}
