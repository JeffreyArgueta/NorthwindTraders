using NorthwindTraders.Infrastructure.Persistence;
using System.Threading;

namespace NorthwindTraders.Infrastructure.Repositories
{
    public abstract class BaseRepository
    {
        protected readonly NorthwindDbContext _db;

        protected BaseRepository(NorthwindDbContext db)
        {
            _db = db;
        }
    }
}
