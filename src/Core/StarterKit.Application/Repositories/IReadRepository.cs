using StarterKit.Domain.Entities.Common;
using System.Linq.Expressions;

namespace StarterKit.Application.Repositories
{
    public interface IReadRepository<T> : IRepository<T> where T : Entity
    {
        IQueryable<T> GetAll(bool tracking = true);
        IQueryable<T> GetWhere(Expression<Func<T, bool>> method, Func<IQueryable<T>, IQueryable<T>> include = null, bool tracking = true);
        Task<T> GetSingleAsync(Expression<Func<T, bool>> method, Func<IQueryable<T>, IQueryable<T>> include = null, bool tracking = true);
        Task<T> GetByIdAsync(string id, Func<IQueryable<T>, IQueryable<T>> include = null, bool tracking = true);
    }
}
