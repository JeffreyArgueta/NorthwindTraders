using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using NorthwindTraders.Infrastructure.Persistence;
using NorthwindTraders.Infrastructure.Repositories;
using NorthwindTraders.Domain.Contracts;

namespace NorthwindTraders.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddNorthwindInfrastructure(this IServiceCollection services, string connectionString)
        {
            services.AddDbContext<NorthwindDbContext>(options => options.UseSqlServer(connectionString));

            // Repositories
            services.AddScoped<IOrderRepository, OrderRepository>();
            services.AddScoped<IOrderDetailRepository, OrderDetailRepository>();
            services.AddScoped<ICustomerRepository, CustomerRepository>();
            services.AddScoped<IEmployeeRepository, EmployeeRepository>();
            services.AddScoped<IShipperRepository, ShipperRepository>();
            services.AddScoped<IProductRepository, ProductRepository>();
            services.AddScoped<ISupplierRepository, SupplierRepository>();

            // Read-only repos
            services.AddScoped<ICategoryRepository, CategoryRepository>();
            services.AddScoped<IRegionRepository, RegionRepository>();
            services.AddScoped<ITerritoryRepository, TerritoryRepository>();
            services.AddScoped<ICustomerDemographicRepository, CustomerDemographicRepository>();
            services.AddScoped<IEmployeeTerritoryRepository, EmployeeTerritoryRepository>();

            return services;
        }
    }
}
