using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using NorthwindTraders.Domain.Contracts;
using NorthwindTraders.Domain.Northwind.Entities;
using NorthwindTraders.Infrastructure.Persistence;

namespace NorthwindTraders.Infrastructure.Repositories
{
    public class CustomerDemographicRepository : BaseRepository, ICustomerDemographicRepository
    {
        public CustomerDemographicRepository(NorthwindDbContext db) : base(db) { }

        public async Task<IEnumerable<CustomerDemographic>> GetAllAsync()
        {
            return await _db.CustomerDemographics.ToListAsync();
        }

        public async Task<CustomerDemographic?> GetByIdAsync(string customerTypeId)
        {
            return await _db.CustomerDemographics.FirstOrDefaultAsync(cd => cd.CustomerTypeID == customerTypeId);
        }
    }
}
