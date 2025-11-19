using StarterKit.Application.Repositories.TemporaryUser;
using StarterKit.Persistence.Contexts;

namespace StarterKit.Persistence.Repositories.TemporaryUser
{
    public class TemporaryUserReadRepository : ReadRepository<Domain.Entities.Identity.TemporaryUser>, ITemporaryUserReadRepository
    {
        public TemporaryUserReadRepository(ApplicationDbContext context) : base(context)
        {
        }
    }
}
