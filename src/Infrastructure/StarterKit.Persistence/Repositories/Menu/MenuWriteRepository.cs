using StarterKit.Application.Repositories.Menu;
using StarterKit.Persistence.Contexts;

namespace StarterKit.Persistence.Repositories.Menu
{
    public class MenuWriteRepository : WriteRepository<Domain.Entities.MenuAggregate.Menu>, IMenuWriteRepository
    {
        public MenuWriteRepository(ApplicationDbContext context) : base(context)
        {
        }
    }
}
