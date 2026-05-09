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
    public class OrderDetailRepository : BaseRepository, IOrderDetailRepository
    {
        public OrderDetailRepository(NorthwindDbContext db) : base(db) { }

        public async Task<PagedResult<OrderDetailDto>> GetByOrderIdAsync(int orderId, int page = 1, int pageSize = 50, CancellationToken ct = default)
        {
            if (page < 1) page = 1;
            if (pageSize < 1) pageSize = 50;
            if (pageSize > 100) pageSize = 100;

            var query = _db.OrderDetails
                .AsNoTracking()
                .Where(od => od.OrderID == orderId)
                .OrderBy(od => od.ProductID)
                .Select(od => new OrderDetailDto(od.OrderID, od.ProductID, od.UnitPrice, od.Quantity, od.Discount));

            var total = await query.CountAsync(ct);
            var items = await query.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync(ct);
            return new PagedResult<OrderDetailDto>(items, total, page, pageSize);
        }

        public async Task<OrderDetailDto?> GetByIdAsync(int orderId, int productId, CancellationToken ct = default)
        {
            var od = await _db.OrderDetails.AsNoTracking().FirstOrDefaultAsync(od => od.OrderID == orderId && od.ProductID == productId, ct);
            if (od == null) return null;
            return new OrderDetailDto(od.OrderID, od.ProductID, od.UnitPrice, od.Quantity, od.Discount);
        }

        public async Task<OrderDetailDto> CreateAsync(int orderId, CreateOrderDetailDto dto, CancellationToken ct = default)
        {
            var od = new OrderDetail { OrderID = orderId, ProductID = dto.ProductID, UnitPrice = dto.UnitPrice, Quantity = dto.Quantity, Discount = dto.Discount };
            await _db.OrderDetails.AddAsync(od, ct);
            await _db.SaveChangesAsync(ct);
            return new OrderDetailDto(od.OrderID, od.ProductID, od.UnitPrice, od.Quantity, od.Discount);
        }

        public async Task<OrderDetailDto> UpdateAsync(int orderId, int productId, UpdateOrderDetailDto dto, CancellationToken ct = default)
        {
            var od = await _db.OrderDetails.AsNoTracking().FirstOrDefaultAsync(x => x.OrderID == orderId && x.ProductID == productId, ct);
            if (od == null) throw new KeyNotFoundException($"OrderDetail {orderId}/{productId} not found");
            od.UnitPrice = dto.UnitPrice;
            od.Quantity = dto.Quantity;
            od.Discount = dto.Discount;
            _db.OrderDetails.Update(od);
            await _db.SaveChangesAsync(ct);
            return new OrderDetailDto(od.OrderID, od.ProductID, od.UnitPrice, od.Quantity, od.Discount);
        }

        public async Task DeleteAsync(int orderId, int productId, CancellationToken ct = default)
        {
            var od = await _db.OrderDetails.FindAsync(new object[] { orderId, productId }, ct);
            if (od == null) throw new KeyNotFoundException($"OrderDetail {orderId}/{productId} not found");
            _db.OrderDetails.Remove(od);
            await _db.SaveChangesAsync(ct);
        }
    }
}
