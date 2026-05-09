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
    public class ProductRepository : BaseRepository, IProductRepository
    {
        public ProductRepository(NorthwindDbContext db) : base(db) { }

        public async Task<PagedResult<ProductDto>> GetAllAsync(int page = 1, int pageSize = 20, CancellationToken ct = default)
        {
            if (page < 1) page = 1;
            if (pageSize < 1) pageSize = 20;
            if (pageSize > 100) pageSize = 100; // enforce upper bound to avoid large responses

            var query = _db.Products
                .AsNoTracking()
                .OrderBy(p => p.ProductID)
                .Select(p => new ProductDto(p.ProductID, p.ProductName, p.SupplierID, p.CategoryID, p.QuantityPerUnit, p.UnitPrice, p.UnitsInStock, p.UnitsOnOrder, p.ReorderLevel, p.Discontinued));

            var total = await query.CountAsync(ct);
            var items = await query.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync(ct);
            return new PagedResult<ProductDto>(items, total, page, pageSize);
        }

        public async Task<ProductDto?> GetByIdAsync(int productId, CancellationToken ct = default)
        {
            var p = await _db.Products.AsNoTracking().FirstOrDefaultAsync(p => p.ProductID == productId, ct);
            if (p == null) return null;
            return new ProductDto(p.ProductID, p.ProductName, p.SupplierID, p.CategoryID, p.QuantityPerUnit, p.UnitPrice, p.UnitsInStock, p.UnitsOnOrder, p.ReorderLevel, p.Discontinued);
        }

        public async Task<ProductDto> CreateAsync(CreateProductDto dto, CancellationToken ct = default)
        {
            var p = new Product { ProductName = dto.ProductName, SupplierID = dto.SupplierID, CategoryID = dto.CategoryID, QuantityPerUnit = dto.QuantityPerUnit, UnitPrice = dto.UnitPrice, UnitsInStock = dto.UnitsInStock, UnitsOnOrder = dto.UnitsOnOrder, ReorderLevel = dto.ReorderLevel, Discontinued = dto.Discontinued };
            _db.Products.Add(p);
            await _db.SaveChangesAsync(ct);
            return new ProductDto(p.ProductID, p.ProductName, p.SupplierID, p.CategoryID, p.QuantityPerUnit, p.UnitPrice, p.UnitsInStock, p.UnitsOnOrder, p.ReorderLevel, p.Discontinued);
        }

        public async Task<ProductDto> UpdateAsync(int productId, UpdateProductDto dto, CancellationToken ct = default)
        {
            var p = await _db.Products.FirstOrDefaultAsync(x => x.ProductID == productId, ct);
            if (p == null) throw new KeyNotFoundException($"Product {productId} not found");
            p.ProductName = dto.ProductName ?? p.ProductName;
            p.SupplierID = dto.SupplierID ?? p.SupplierID;
            p.CategoryID = dto.CategoryID ?? p.CategoryID;
            p.QuantityPerUnit = dto.QuantityPerUnit ?? p.QuantityPerUnit;
            p.UnitPrice = dto.UnitPrice ?? p.UnitPrice;
            p.UnitsInStock = dto.UnitsInStock ?? p.UnitsInStock;
            p.UnitsOnOrder = dto.UnitsOnOrder ?? p.UnitsOnOrder;
            p.ReorderLevel = dto.ReorderLevel ?? p.ReorderLevel;
            p.Discontinued = dto.Discontinued ?? p.Discontinued;
            _db.Products.Update(p);
            await _db.SaveChangesAsync(ct);
            return new ProductDto(p.ProductID, p.ProductName, p.SupplierID, p.CategoryID, p.QuantityPerUnit, p.UnitPrice, p.UnitsInStock, p.UnitsOnOrder, p.ReorderLevel, p.Discontinued);
        }

        public async Task DeleteAsync(int productId, CancellationToken ct = default)
        {
            var p = await _db.Products.FindAsync(new object[] { productId }, ct);
            if (p == null) throw new KeyNotFoundException($"Product {productId} not found");
            _db.Products.Remove(p);
            await _db.SaveChangesAsync(ct);
        }
    }
}
