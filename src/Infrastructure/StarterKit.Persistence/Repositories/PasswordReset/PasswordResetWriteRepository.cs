using StarterKit.Application.Repositories.PasswordReset;
using StarterKit.Persistence.Contexts;

namespace StarterKit.Persistence.Repositories.PasswordReset
{
    public class PasswordResetWriteRepository : WriteRepository<Domain.Entities.Identity.PasswordReset>, IPasswordResetWriteRepository
    {
        public PasswordResetWriteRepository(ApplicationDbContext context) : base(context)
        {
        }
    }
}