using StarterKit.Domain.Entities.Common;

namespace StarterKit.Application.Repositories
{
    public interface IWriteRepository<T> : IRepository<T> where T : Entity
    {
        Task<bool> AddAsync(T datas);
        Task<bool> AddRangeAsync(List<T> datas);
        bool Remove(T datas);
        bool RemoveRange(List<T> datas);
        Task<bool> RemoveAsync(int id);
        bool Update(T model);
        Task<int> SaveAsync();
    }
}
