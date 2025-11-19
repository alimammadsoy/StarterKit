using StarterKit.Application.Repositories.TemporaryUser;
using StarterKit.Persistence.Contexts;

namespace StarterKit.Persistence.Repositories.TemporaryUser
{
    public class TemporaryUserWriteRepository : WriteRepository<Domain.Entities.Identity.TemporaryUser>, ITemporaryUserWriteRepository
    {
        public TemporaryUserWriteRepository(ApplicationDbContext context) : base(context)
        {
        }
    }
}
