using StarterKit.Application.Repositories.PasswordReset;
using StarterKit.Persistence.Contexts;

namespace StarterKit.Persistence.Repositories.PasswordReset
{
    public class PasswordResetReadRepository : ReadRepository<Domain.Entities.Identity.PasswordReset>, IPasswordResetReadRepository
    {
        public PasswordResetReadRepository(ApplicationDbContext context) : base(context)
        {
        }
    }
}