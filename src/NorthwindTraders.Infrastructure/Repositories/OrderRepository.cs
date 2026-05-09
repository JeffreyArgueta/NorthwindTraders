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
    public class OrderRepository : BaseRepository, IOrderRepository
    {
        public OrderRepository(NorthwindDbContext db) : base(db) { }

        public async Task<PagedResult<OrderDto>> GetAllAsync(int page = 1, int pageSize = 20, CancellationToken ct = default)
        {
            if (page < 1) page = 1;
            if (pageSize < 1) pageSize = 20;

            var query = _db.Orders
                .AsNoTracking()
                .OrderBy(o => o.OrderID)
                .Select(o => new OrderDto(
                    o.OrderID,
                    o.CustomerID,
                    o.EmployeeID,
                    o.OrderDate,
                    o.RequiredDate,
                    o.ShippedDate,
                    o.ShipVia,
                    o.Freight,
                    o.ShipName,
                    o.ShipAddress,
                    o.ShipCity,
                    o.ShipRegion,
                    o.ShipPostalCode,
                    o.ShipCountry
                ));

            if (pageSize > 100) pageSize = 100;
            var total = await query.CountAsync(ct);
            var items = await query.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync(ct);
            return new PagedResult<OrderDto>(items, total, page, pageSize);
        }

        public async Task<OrderDto?> GetByIdAsync(int orderId, CancellationToken ct = default)
        {
            var o = await _db.Orders
                .Include(o => o.OrderDetails)
                .Include(o => o.Customer)
                .Include(o => o.Employee)
                .Include(o => o.Shipper)
                .AsNoTracking()
                .FirstOrDefaultAsync(o => o.OrderID == orderId, ct);

            if (o == null) return null;

            var orderDetails = o.OrderDetails.Select(od => new OrderDetailDto(od.OrderID, od.ProductID, od.UnitPrice, od.Quantity, od.Discount));
            var customer = o.Customer != null ? new CustomerDto(o.Customer.CustomerID, o.Customer.CompanyName, o.Customer.ContactName, o.Customer.ContactTitle) : null;
            var employee = o.Employee != null ? new EmployeeDto(o.Employee.EmployeeID, o.Employee.FirstName, o.Employee.LastName, o.Employee.Title) : null;
            var shipper = o.Shipper != null ? new ShipperDto(o.Shipper.ShipperID, o.Shipper.CompanyName, o.Shipper.Phone) : null;

            return new OrderDto(
                o.OrderID,
                o.CustomerID,
                o.EmployeeID,
                o.OrderDate,
                o.RequiredDate,
                o.ShippedDate,
                o.ShipVia,
                o.Freight,
                o.ShipName,
                o.ShipAddress,
                o.ShipCity,
                o.ShipRegion,
                o.ShipPostalCode,
                o.ShipCountry,
                customer,
                employee,
                shipper,
                orderDetails
            );
        }

        public async Task<OrderDto> CreateAsync(CreateOrderDto dto, CancellationToken ct = default)
        {
            var order = new Order
            {
                CustomerID = dto.CustomerID,
                EmployeeID = dto.EmployeeID,
                OrderDate = dto.OrderDate,
                RequiredDate = dto.RequiredDate,
                ShippedDate = dto.ShippedDate,
                ShipVia = dto.ShipVia,
                Freight = dto.Freight,
                ShipName = dto.ShipName,
                ShipAddress = dto.ShipAddress,
                ShipCity = dto.ShipCity,
                ShipRegion = dto.ShipRegion,
                ShipPostalCode = dto.ShipPostalCode,
                ShipCountry = dto.ShipCountry
            };

            if (dto.OrderDetails != null)
            {
                foreach (var d in dto.OrderDetails)
                {
                    order.OrderDetails.Add(new OrderDetail { ProductID = d.ProductID, UnitPrice = d.UnitPrice, Quantity = d.Quantity, Discount = d.Discount });
                }
            }

            await _db.Orders.AddAsync(order, ct);
            await _db.SaveChangesAsync(ct);

            return await GetByIdAsync(order.OrderID, ct) ?? throw new System.InvalidOperationException("Failed to fetch created order");
        }

        public async Task<OrderDto> UpdateAsync(int orderId, UpdateOrderDto dto, CancellationToken ct = default)
        {
            var order = await _db.Orders.Include(o => o.OrderDetails).FirstOrDefaultAsync(o => o.OrderID == orderId, ct);
            if (order == null) throw new KeyNotFoundException($"Order with ID {orderId} not found");

            order.CustomerID = dto.CustomerID;
            order.EmployeeID = dto.EmployeeID;
            order.OrderDate = dto.OrderDate;
            order.RequiredDate = dto.RequiredDate;
            order.ShippedDate = dto.ShippedDate;
            order.ShipVia = dto.ShipVia;
            order.Freight = dto.Freight;
            order.ShipName = dto.ShipName;
            order.ShipAddress = dto.ShipAddress;
            order.ShipCity = dto.ShipCity;
            order.ShipRegion = dto.ShipRegion;
            order.ShipPostalCode = dto.ShipPostalCode;
            order.ShipCountry = dto.ShipCountry;

            _db.Orders.Update(order);
            await _db.SaveChangesAsync(ct);

            return await GetByIdAsync(orderId, ct) ?? throw new System.InvalidOperationException("Failed to fetch updated order");
        }

        public async Task DeleteAsync(int orderId, CancellationToken ct = default)
        {
            var existing = await _db.Orders.FindAsync(new object[] { orderId }, ct);
            if (existing == null) throw new KeyNotFoundException($"Order with ID {orderId} not found");
            _db.Orders.Remove(existing);
            await _db.SaveChangesAsync(ct);
        }
    }
}
