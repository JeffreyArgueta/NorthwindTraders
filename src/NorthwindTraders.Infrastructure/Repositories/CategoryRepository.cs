using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using NorthwindTraders.Domain.Contracts;
using NorthwindTraders.Domain.Northwind.Entities;
using NorthwindTraders.Infrastructure.Persistence;

namespace NorthwindTraders.Infrastructure.Repositories
{
    public class CategoryRepository : BaseRepository, ICategoryRepository
    {
        public CategoryRepository(NorthwindDbContext db) : base(db) { }

        public async Task<IEnumerable<Category>> GetAllAsync()
        {
            return await _db.Categories.ToListAsync();
        }

        public async Task<Category?> GetByIdAsync(int categoryId)
        {
            return await _db.Categories.FirstOrDefaultAsync(c => c.CategoryID == categoryId);
        }
    }
}
