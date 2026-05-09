using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using NorthwindTraders.Domain.Contracts;
using NorthwindTraders.Domain.Northwind.Entities;
using NorthwindTraders.Infrastructure.Persistence;

namespace NorthwindTraders.Infrastructure.Repositories
{
    public class TerritoryRepository : BaseRepository, ITerritoryRepository
    {
        public TerritoryRepository(NorthwindDbContext db) : base(db) { }

        public async Task<IEnumerable<Territory>> GetAllAsync()
        {
            return await _db.Territories.ToListAsync();
        }

        public async Task<Territory?> GetByIdAsync(string territoryId)
        {
            return await _db.Territories.FirstOrDefaultAsync(t => t.TerritoryID == territoryId);
        }
    }
}
