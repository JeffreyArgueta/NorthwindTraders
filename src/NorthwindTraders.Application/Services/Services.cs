using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using NorthwindTraders.Domain.Contracts;

namespace NorthwindTraders.Application.Services
{
    public sealed class OrderService : IOrderService
    {
        private readonly IOrderRepository _repo;

        public OrderService(IOrderRepository repo)
        {
            _repo = repo;
        }

        public async Task<PagedResult<OrderDto>> GetAllAsync(int page = 1, int pageSize = 20, CancellationToken ct = default)
        {
            return await _repo.GetAllAsync(page, pageSize, ct);
        }

        public async Task<OrderDto?> GetByIdAsync(int orderId, CancellationToken ct = default)
        {
            return await _repo.GetByIdAsync(orderId, ct);
        }

        public async Task<OrderDto> CreateAsync(CreateOrderDto dto, CancellationToken ct = default)
        {
            return await _repo.CreateAsync(dto, ct);
        }

        public async Task<OrderDto> UpdateAsync(int orderId, UpdateOrderDto dto, CancellationToken ct = default)
        {
            return await _repo.UpdateAsync(orderId, dto, ct);
        }

        public async Task DeleteAsync(int orderId, CancellationToken ct = default)
        {
            await _repo.DeleteAsync(orderId, ct);
        }
    }

    public sealed class OrderDetailService : IOrderDetailService
    {
        private readonly IOrderDetailRepository _repo;

        public OrderDetailService(IOrderDetailRepository repo)
        {
            _repo = repo;
        }

        public async Task<PagedResult<OrderDetailDto>> GetByOrderIdAsync(int orderId, int page = 1, int pageSize = 50, CancellationToken ct = default)
        {
            return await _repo.GetByOrderIdAsync(orderId, page, pageSize, ct);
        }

        public async Task<OrderDetailDto?> GetByIdAsync(int orderId, int productId, CancellationToken ct = default)
        {
            return await _repo.GetByIdAsync(orderId, productId, ct);
        }

        public async Task<OrderDetailDto> CreateAsync(int orderId, CreateOrderDetailDto dto, CancellationToken ct = default)
        {
            return await _repo.CreateAsync(orderId, dto, ct);
        }

        public async Task<OrderDetailDto> UpdateAsync(int orderId, int productId, UpdateOrderDetailDto dto, CancellationToken ct = default)
        {
            return await _repo.UpdateAsync(orderId, productId, dto, ct);
        }

        public async Task DeleteAsync(int orderId, int productId, CancellationToken ct = default)
        {
            await _repo.DeleteAsync(orderId, productId, ct);
        }
    }

    public sealed class CustomerService : ICustomerService
    {
        private readonly ICustomerRepository _repo;

        public CustomerService(ICustomerRepository repo)
        {
            _repo = repo;
        }

        public async Task<PagedResult<CustomerDto>> GetAllAsync(int page = 1, int pageSize = 20, CancellationToken ct = default)
        {
            return await _repo.GetAllAsync(page, pageSize, ct);
        }

        public async Task<CustomerDto?> GetByIdAsync(string customerId, CancellationToken ct = default)
        {
            return await _repo.GetByIdAsync(customerId, ct);
        }

        public async Task<CustomerDto> CreateAsync(CreateCustomerDto dto, CancellationToken ct = default)
        {
            return await _repo.CreateAsync(dto, ct);
        }

        public async Task<CustomerDto> UpdateAsync(string customerId, UpdateCustomerDto dto, CancellationToken ct = default)
        {
            return await _repo.UpdateAsync(customerId, dto, ct);
        }

        public async Task DeleteAsync(string customerId, CancellationToken ct = default)
        {
            await _repo.DeleteAsync(customerId, ct);
        }
    }

    public sealed class EmployeeService : IEmployeeService
    {
        private readonly IEmployeeRepository _repo;

        public EmployeeService(IEmployeeRepository repo)
        {
            _repo = repo;
        }

        public async Task<PagedResult<EmployeeDto>> GetAllAsync(int page = 1, int pageSize = 20, CancellationToken ct = default)
        {
            return await _repo.GetAllAsync(page, pageSize, ct);
        }

        public async Task<EmployeeDto?> GetByIdAsync(int employeeId, CancellationToken ct = default)
        {
            return await _repo.GetByIdAsync(employeeId, ct);
        }

        public async Task<EmployeeDto> CreateAsync(CreateEmployeeDto dto, CancellationToken ct = default)
        {
            return await _repo.CreateAsync(dto, ct);
        }

        public async Task<EmployeeDto> UpdateAsync(int employeeId, UpdateEmployeeDto dto, CancellationToken ct = default)
        {
            return await _repo.UpdateAsync(employeeId, dto, ct);
        }

        public async Task DeleteAsync(int employeeId, CancellationToken ct = default)
        {
            await _repo.DeleteAsync(employeeId, ct);
        }
    }

    public sealed class ShipperService : IShipperService
    {
        private readonly IShipperRepository _repo;

        public ShipperService(IShipperRepository repo)
        {
            _repo = repo;
        }

        public async Task<PagedResult<ShipperDto>> GetAllAsync(int page = 1, int pageSize = 20, CancellationToken ct = default)
        {
            return await _repo.GetAllAsync(page, pageSize, ct);
        }

        public async Task<ShipperDto?> GetByIdAsync(int shipperId, CancellationToken ct = default)
        {
            return await _repo.GetByIdAsync(shipperId, ct);
        }

        public async Task<ShipperDto> CreateAsync(CreateShipperDto dto, CancellationToken ct = default)
        {
            return await _repo.CreateAsync(dto, ct);
        }

        public async Task<ShipperDto> UpdateAsync(int shipperId, UpdateShipperDto dto, CancellationToken ct = default)
        {
            return await _repo.UpdateAsync(shipperId, dto, ct);
        }

        public async Task DeleteAsync(int shipperId, CancellationToken ct = default)
        {
            await _repo.DeleteAsync(shipperId, ct);
        }
    }

    public sealed class ProductService : IProductService
    {
        private readonly IProductRepository _repo;

        public ProductService(IProductRepository repo)
        {
            _repo = repo;
        }

        public async Task<PagedResult<ProductDto>> GetAllAsync(int page = 1, int pageSize = 20, CancellationToken ct = default)
        {
            return await _repo.GetAllAsync(page, pageSize, ct);
        }

        public async Task<ProductDto?> GetByIdAsync(int productId, CancellationToken ct = default)
        {
            return await _repo.GetByIdAsync(productId, ct);
        }

        public async Task<ProductDto> CreateAsync(CreateProductDto dto, CancellationToken ct = default)
        {
            return await _repo.CreateAsync(dto, ct);
        }

        public async Task<ProductDto> UpdateAsync(int productId, UpdateProductDto dto, CancellationToken ct = default)
        {
            return await _repo.UpdateAsync(productId, dto, ct);
        }

        public async Task DeleteAsync(int productId, CancellationToken ct = default)
        {
            await _repo.DeleteAsync(productId, ct);
        }
    }

    public sealed class SupplierService : ISupplierService
    {
        private readonly ISupplierRepository _repo;

        public SupplierService(ISupplierRepository repo)
        {
            _repo = repo;
        }

        public async Task<PagedResult<SupplierDto>> GetAllAsync(int page = 1, int pageSize = 20, CancellationToken ct = default)
        {
            return await _repo.GetAllAsync(page, pageSize, ct);
        }

        public async Task<SupplierDto?> GetByIdAsync(int supplierId, CancellationToken ct = default)
        {
            return await _repo.GetByIdAsync(supplierId, ct);
        }

        public async Task<SupplierDto> CreateAsync(CreateSupplierDto dto, CancellationToken ct = default)
        {
            return await _repo.CreateAsync(dto, ct);
        }

        public async Task<SupplierDto> UpdateAsync(int supplierId, UpdateSupplierDto dto, CancellationToken ct = default)
        {
            return await _repo.UpdateAsync(supplierId, dto, ct);
        }

        public async Task DeleteAsync(int supplierId, CancellationToken ct = default)
        {
            await _repo.DeleteAsync(supplierId, ct);
        }
    }

    // Read-only lookup services
    public sealed class CategoryService : ICategoryService
    {
        private readonly ICategoryRepository _repo;

        public CategoryService(ICategoryRepository repo) => _repo = repo;

        public async Task<IEnumerable<CategoryDto>> GetAllAsync()
        {
            var list = await Task.Run(() => _repo.GetAllAsync());
            // map to DTOs
            var dtos = new List<CategoryDto>();
            foreach (var c in list)
                dtos.Add(new CategoryDto(c.CategoryID, c.CategoryName, c.Description));
            return dtos;
        }

        public async Task<CategoryDto?> GetByIdAsync(int categoryId)
        {
            var c = await Task.Run(() => _repo.GetByIdAsync(categoryId));
            if (c == null) return null;
            return new CategoryDto(c.CategoryID, c.CategoryName, c.Description);
        }
    }

    public sealed class RegionService : IRegionService
    {
        private readonly IRegionRepository _repo;
        public RegionService(IRegionRepository repo) => _repo = repo;

        public async Task<IEnumerable<RegionDto>> GetAllAsync()
        {
            var list = await Task.Run(() => _repo.GetAllAsync());
            var dtos = new List<RegionDto>();
            foreach (var r in list)
                dtos.Add(new RegionDto(r.RegionID, r.RegionDescription));
            return dtos;
        }

        public async Task<RegionDto?> GetByIdAsync(int regionId)
        {
            var r = await Task.Run(() => _repo.GetByIdAsync(regionId));
            if (r == null) return null;
            return new RegionDto(r.RegionID, r.RegionDescription);
        }
    }

    public sealed class TerritoryService : ITerritoryService
    {
        private readonly ITerritoryRepository _repo;
        public TerritoryService(ITerritoryRepository repo) => _repo = repo;

        public async Task<IEnumerable<TerritoryDto>> GetAllAsync()
        {
            var list = await Task.Run(() => _repo.GetAllAsync());
            var dtos = new List<TerritoryDto>();
            foreach (var t in list)
                dtos.Add(new TerritoryDto(t.TerritoryID, t.TerritoryDescription, t.RegionID));
            return dtos;
        }

        public async Task<TerritoryDto?> GetByIdAsync(string territoryId)
        {
            var t = await Task.Run(() => _repo.GetByIdAsync(territoryId));
            if (t == null) return null;
            return new TerritoryDto(t.TerritoryID, t.TerritoryDescription, t.RegionID);
        }
    }

    public sealed class CustomerDemographicService : ICustomerDemographicService
    {
        private readonly ICustomerDemographicRepository _repo;
        public CustomerDemographicService(ICustomerDemographicRepository repo) => _repo = repo;

        public async Task<IEnumerable<CustomerDemographicDto>> GetAllAsync()
        {
            var list = await Task.Run(() => _repo.GetAllAsync());
            var dtos = new List<CustomerDemographicDto>();
            foreach (var d in list)
                dtos.Add(new CustomerDemographicDto(d.CustomerTypeID, d.CustomerDesc));
            return dtos;
        }

        public async Task<CustomerDemographicDto?> GetByIdAsync(string customerTypeId)
        {
            var d = await Task.Run(() => _repo.GetByIdAsync(customerTypeId));
            if (d == null) return null;
            return new CustomerDemographicDto(d.CustomerTypeID, d.CustomerDesc);
        }
    }

    public sealed class EmployeeTerritoryService : IEmployeeTerritoryService
    {
        private readonly IEmployeeTerritoryRepository _repo;
        public EmployeeTerritoryService(IEmployeeTerritoryRepository repo) => _repo = repo;

        public async Task<IEnumerable<EmployeeTerritoryDto>> GetByEmployeeIdAsync(int employeeId)
        {
            var list = await _repo.GetByEmployeeIdAsync(employeeId);
            var dtos = new List<EmployeeTerritoryDto>();
            foreach (var et in list)
                dtos.Add(new EmployeeTerritoryDto(et.EmployeeID, et.TerritoryID));
            return dtos;
        }
    }
}
