using StarterKit.Application.Repositories.Menu;
using StarterKit.Persistence.Contexts;

namespace StarterKit.Persistence.Repositories.Menu
{
    public class MenuReadRepository : ReadRepository<Domain.Entities.MenuAggregate.Menu>, IMenuReadRepository
    {
        public MenuReadRepository(ApplicationDbContext context) : base(context)
        {
        }
    }
}
