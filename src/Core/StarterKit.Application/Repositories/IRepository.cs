using Microsoft.EntityFrameworkCore;
using StarterKit.Domain.Entities.Common;

namespace StarterKit.Application.Repositories
{
    public interface IRepository<T> where T : Entity
    {
        DbSet<T> Table { get; }
    }
}
