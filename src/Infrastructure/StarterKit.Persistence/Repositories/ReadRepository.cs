using Microsoft.EntityFrameworkCore;
using StarterKit.Application.Repositories;
using StarterKit.Domain.Entities.Common;
using StarterKit.Persistence.Contexts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace StarterKit.Persistence.Repositories
{
    public class ReadRepository<T> : IReadRepository<T> where T : Entity
    {
        private readonly ApplicationDbContext _context;

        public ReadRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public DbSet<T> Table => _context.Set<T>();

        public IQueryable<T> GetAll(bool tracking = true)
        {
            var query = Table.AsQueryable();
            if (!tracking)
                query = query.AsNoTracking();
            return query;
        }
        public IQueryable<T> GetWhere(Expression<Func<T, bool>> method, Func<IQueryable<T>, IQueryable<T>> include = null, bool tracking = true)
        {
            var query = Table.Where(method).OrderByDescending(o => o.CreatedDate).AsQueryable();
            if (!tracking)
                query = query.AsNoTracking();
            if (include is not null)
                query = include(query);
            return query;
        }
        public async Task<T> GetSingleAsync(Expression<Func<T, bool>> method, Func<IQueryable<T>, IQueryable<T>> include = null, bool tracking = true)
        {
            var query = Table.AsQueryable();
            if (!tracking)
                query = query.AsNoTracking();
            if (include is not null)
                query = include(query);

            return await query.FirstOrDefaultAsync(method);
        }

        public async Task<T> GetByIdAsync(string id, Func<IQueryable<T>, IQueryable<T>> include = null, bool tracking = true)
        {
            var query = Table.AsQueryable();
            if (!tracking)
                query = Table.AsNoTracking();
            if (include is not null)
                query = include(query);

            return await query.FirstOrDefaultAsync(data => data.Id == Guid.Parse(id) && !data.IsDeleted);
        }
    }
}
