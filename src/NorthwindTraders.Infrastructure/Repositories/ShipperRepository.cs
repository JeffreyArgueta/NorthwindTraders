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
    public class ShipperRepository : BaseRepository, IShipperRepository
    {
        public ShipperRepository(NorthwindDbContext db) : base(db) { }

        public async Task<PagedResult<ShipperDto>> GetAllAsync(int page = 1, int pageSize = 20, CancellationToken ct = default)
        {
            if (page < 1) page = 1;
            if (pageSize < 1) pageSize = 20;

            var query = _db.Shippers
                .AsNoTracking()
                .OrderBy(s => s.ShipperID)
                .Select(s => new ShipperDto(s.ShipperID, s.CompanyName, s.Phone));
            var total = await query.CountAsync(ct);
            var items = await query.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync(ct);
            return new PagedResult<ShipperDto>(items, total, page, pageSize);
        }

        public async Task<ShipperDto?> GetByIdAsync(int shipperId, CancellationToken ct = default)
        {
            var s = await _db.Shippers.AsNoTracking().FirstOrDefaultAsync(s => s.ShipperID == shipperId, ct);
            if (s == null) return null;
            return new ShipperDto(s.ShipperID, s.CompanyName, s.Phone);
        }

        public async Task<ShipperDto> CreateAsync(CreateShipperDto dto, CancellationToken ct = default)
        {
            var s = new Shipper { CompanyName = dto.CompanyName, Phone = dto.Phone };
            _db.Shippers.Add(s);
            await _db.SaveChangesAsync(ct);
            return new ShipperDto(s.ShipperID, s.CompanyName, s.Phone);
        }

        public async Task<ShipperDto> UpdateAsync(int shipperId, UpdateShipperDto dto, CancellationToken ct = default)
        {
            var s = await _db.Shippers.FirstOrDefaultAsync(x => x.ShipperID == shipperId, ct);
            if (s == null) throw new KeyNotFoundException($"Shipper {shipperId} not found");
            s.CompanyName = dto.CompanyName ?? s.CompanyName;
            s.Phone = dto.Phone ?? s.Phone;
            _db.Shippers.Update(s);
            await _db.SaveChangesAsync(ct);
            return new ShipperDto(s.ShipperID, s.CompanyName, s.Phone);
        }

        public async Task DeleteAsync(int shipperId, CancellationToken ct = default)
        {
            var s = await _db.Shippers.FindAsync(new object[] { shipperId }, ct);
            if (s == null) throw new KeyNotFoundException($"Shipper {shipperId} not found");
            _db.Shippers.Remove(s);
            await _db.SaveChangesAsync(ct);
        }
    }
}
