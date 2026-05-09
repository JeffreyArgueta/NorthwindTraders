using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using NorthwindTraders.Domain.Northwind.Entities;

namespace NorthwindTraders.Domain.Contracts
{
    // Paged result
    public record PagedResult<T>(IEnumerable<T> Items, int TotalCount, int Page, int PageSize);

    // Repository interfaces
    public interface IOrderRepository
    {
        Task<PagedResult<OrderDto>> GetAllAsync(int page = 1, int pageSize = 20, CancellationToken ct = default);
        Task<OrderDto?> GetByIdAsync(int orderId, CancellationToken ct = default);
        Task<OrderDto> CreateAsync(CreateOrderDto dto, CancellationToken ct = default);
        Task<OrderDto> UpdateAsync(int orderId, UpdateOrderDto dto, CancellationToken ct = default);
        Task DeleteAsync(int orderId, CancellationToken ct = default);
    }

    public interface IOrderDetailRepository
    {
        Task<PagedResult<OrderDetailDto>> GetByOrderIdAsync(int orderId, int page = 1, int pageSize = 50, CancellationToken ct = default);
        Task<OrderDetailDto?> GetByIdAsync(int orderId, int productId, CancellationToken ct = default);
        Task<OrderDetailDto> CreateAsync(int orderId, CreateOrderDetailDto dto, CancellationToken ct = default);
        Task<OrderDetailDto> UpdateAsync(int orderId, int productId, UpdateOrderDetailDto dto, CancellationToken ct = default);
        Task DeleteAsync(int orderId, int productId, CancellationToken ct = default);
    }

    public interface ICustomerRepository
    {
        Task<PagedResult<CustomerDto>> GetAllAsync(int page = 1, int pageSize = 20, CancellationToken ct = default);
        Task<CustomerDto?> GetByIdAsync(string customerId, CancellationToken ct = default);
        Task<CustomerDto> CreateAsync(CreateCustomerDto dto, CancellationToken ct = default);
        Task<CustomerDto> UpdateAsync(string customerId, UpdateCustomerDto dto, CancellationToken ct = default);
        Task DeleteAsync(string customerId, CancellationToken ct = default);
    }

    public interface IEmployeeRepository
    {
        Task<PagedResult<EmployeeDto>> GetAllAsync(int page = 1, int pageSize = 20, CancellationToken ct = default);
        Task<EmployeeDto?> GetByIdAsync(int employeeId, CancellationToken ct = default);
        Task<EmployeeDto> CreateAsync(CreateEmployeeDto dto, CancellationToken ct = default);
        Task<EmployeeDto> UpdateAsync(int employeeId, UpdateEmployeeDto dto, CancellationToken ct = default);
        Task DeleteAsync(int employeeId, CancellationToken ct = default);
    }

    public interface IShipperRepository
    {
        Task<PagedResult<ShipperDto>> GetAllAsync(int page = 1, int pageSize = 20, CancellationToken ct = default);
        Task<ShipperDto?> GetByIdAsync(int shipperId, CancellationToken ct = default);
        Task<ShipperDto> CreateAsync(CreateShipperDto dto, CancellationToken ct = default);
        Task<ShipperDto> UpdateAsync(int shipperId, UpdateShipperDto dto, CancellationToken ct = default);
        Task DeleteAsync(int shipperId, CancellationToken ct = default);
    }

    public interface IProductRepository
    {
        Task<PagedResult<ProductDto>> GetAllAsync(int page = 1, int pageSize = 20, CancellationToken ct = default);
        Task<ProductDto?> GetByIdAsync(int productId, CancellationToken ct = default);
        Task<ProductDto> CreateAsync(CreateProductDto dto, CancellationToken ct = default);
        Task<ProductDto> UpdateAsync(int productId, UpdateProductDto dto, CancellationToken ct = default);
        Task DeleteAsync(int productId, CancellationToken ct = default);
    }

    public interface ISupplierRepository
    {
        Task<PagedResult<SupplierDto>> GetAllAsync(int page = 1, int pageSize = 20, CancellationToken ct = default);
        Task<SupplierDto?> GetByIdAsync(int supplierId, CancellationToken ct = default);
        Task<SupplierDto> CreateAsync(CreateSupplierDto dto, CancellationToken ct = default);
        Task<SupplierDto> UpdateAsync(int supplierId, UpdateSupplierDto dto, CancellationToken ct = default);
        Task DeleteAsync(int supplierId, CancellationToken ct = default);
    }

    // Read-only repository interfaces for lookup tables
    public interface ICategoryRepository
    {
        Task<Category?> GetByIdAsync(int categoryId);
        Task<IEnumerable<Category>> GetAllAsync();
    }

    public interface IRegionRepository
    {
        Task<Region?> GetByIdAsync(int regionId);
        Task<IEnumerable<Region>> GetAllAsync();
    }

    public interface ITerritoryRepository
    {
        Task<Territory?> GetByIdAsync(string territoryId);
        Task<IEnumerable<Territory>> GetAllAsync();
    }

    public interface ICustomerDemographicRepository
    {
        Task<CustomerDemographic?> GetByIdAsync(string customerTypeId);
        Task<IEnumerable<CustomerDemographic>> GetAllAsync();
    }

    public interface IEmployeeTerritoryRepository
    {
        Task<IEnumerable<EmployeeTerritory>> GetByEmployeeIdAsync(int employeeId);
    }

    // Service interfaces (async, cancellation & paging-aware)
    public interface IOrderService
    {
        Task<PagedResult<OrderDto>> GetAllAsync(int page = 1, int pageSize = 20, CancellationToken ct = default);
        Task<OrderDto?> GetByIdAsync(int orderId, CancellationToken ct = default);
        Task<OrderDto> CreateAsync(CreateOrderDto dto, CancellationToken ct = default);
        Task<OrderDto> UpdateAsync(int orderId, UpdateOrderDto dto, CancellationToken ct = default);
        Task DeleteAsync(int orderId, CancellationToken ct = default);
    }

    public interface IOrderDetailService
    {
        Task<PagedResult<OrderDetailDto>> GetByOrderIdAsync(int orderId, int page = 1, int pageSize = 50, CancellationToken ct = default);
        Task<OrderDetailDto?> GetByIdAsync(int orderId, int productId, CancellationToken ct = default);
        Task<OrderDetailDto> CreateAsync(int orderId, CreateOrderDetailDto dto, CancellationToken ct = default);
        Task<OrderDetailDto> UpdateAsync(int orderId, int productId, UpdateOrderDetailDto dto, CancellationToken ct = default);
        Task DeleteAsync(int orderId, int productId, CancellationToken ct = default);
    }

    // Service contracts for core entities
    public interface ICustomerService
    {
        Task<PagedResult<CustomerDto>> GetAllAsync(int page = 1, int pageSize = 20, CancellationToken ct = default);
        Task<CustomerDto?> GetByIdAsync(string customerId, CancellationToken ct = default);
        Task<CustomerDto> CreateAsync(CreateCustomerDto dto, CancellationToken ct = default);
        Task<CustomerDto> UpdateAsync(string customerId, UpdateCustomerDto dto, CancellationToken ct = default);
        Task DeleteAsync(string customerId, CancellationToken ct = default);
    }

    public interface IEmployeeService
    {
        Task<PagedResult<EmployeeDto>> GetAllAsync(int page = 1, int pageSize = 20, CancellationToken ct = default);
        Task<EmployeeDto?> GetByIdAsync(int employeeId, CancellationToken ct = default);
        Task<EmployeeDto> CreateAsync(CreateEmployeeDto dto, CancellationToken ct = default);
        Task<EmployeeDto> UpdateAsync(int employeeId, UpdateEmployeeDto dto, CancellationToken ct = default);
        Task DeleteAsync(int employeeId, CancellationToken ct = default);
    }

    public interface IShipperService
    {
        Task<PagedResult<ShipperDto>> GetAllAsync(int page = 1, int pageSize = 20, CancellationToken ct = default);
        Task<ShipperDto?> GetByIdAsync(int shipperId, CancellationToken ct = default);
        Task<ShipperDto> CreateAsync(CreateShipperDto dto, CancellationToken ct = default);
        Task<ShipperDto> UpdateAsync(int shipperId, UpdateShipperDto dto, CancellationToken ct = default);
        Task DeleteAsync(int shipperId, CancellationToken ct = default);
    }

    public interface IProductService
    {
        Task<PagedResult<ProductDto>> GetAllAsync(int page = 1, int pageSize = 20, CancellationToken ct = default);
        Task<ProductDto?> GetByIdAsync(int productId, CancellationToken ct = default);
        Task<ProductDto> CreateAsync(CreateProductDto dto, CancellationToken ct = default);
        Task<ProductDto> UpdateAsync(int productId, UpdateProductDto dto, CancellationToken ct = default);
        Task DeleteAsync(int productId, CancellationToken ct = default);
    }

    public interface ISupplierService
    {
        Task<PagedResult<SupplierDto>> GetAllAsync(int page = 1, int pageSize = 20, CancellationToken ct = default);
        Task<SupplierDto?> GetByIdAsync(int supplierId, CancellationToken ct = default);
        Task<SupplierDto> CreateAsync(CreateSupplierDto dto, CancellationToken ct = default);
        Task<SupplierDto> UpdateAsync(int supplierId, UpdateSupplierDto dto, CancellationToken ct = default);
        Task DeleteAsync(int supplierId, CancellationToken ct = default);
    }

    // Read-only services for lookup tables
    public interface ICategoryService
    {
        Task<CategoryDto?> GetByIdAsync(int categoryId);
        Task<IEnumerable<CategoryDto>> GetAllAsync();
    }

    public interface IRegionService
    {
        Task<RegionDto?> GetByIdAsync(int regionId);
        Task<IEnumerable<RegionDto>> GetAllAsync();
    }

    public interface ITerritoryService
    {
        Task<TerritoryDto?> GetByIdAsync(string territoryId);
        Task<IEnumerable<TerritoryDto>> GetAllAsync();
    }

    public interface ICustomerDemographicService
    {
        Task<CustomerDemographicDto?> GetByIdAsync(string customerTypeId);
        Task<IEnumerable<CustomerDemographicDto>> GetAllAsync();
    }

    public interface IEmployeeTerritoryService
    {
        Task<IEnumerable<EmployeeTerritoryDto>> GetByEmployeeIdAsync(int employeeId);
    }
}
