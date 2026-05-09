using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using NorthwindTraders.Domain.Contracts;
using NorthwindTraders.Domain.Northwind.Entities;
using NorthwindTraders.Infrastructure.Persistence;

namespace NorthwindTraders.Infrastructure.Repositories
{
    public class EmployeeTerritoryRepository : BaseRepository, IEmployeeTerritoryRepository
    {
        public EmployeeTerritoryRepository(NorthwindDbContext db) : base(db) { }

        public async Task<IEnumerable<EmployeeTerritory>> GetByEmployeeIdAsync(int employeeId)
        {
            return await _db.EmployeeTerritories.Where(et => et.EmployeeID == employeeId).ToListAsync();
        }
    }
}
