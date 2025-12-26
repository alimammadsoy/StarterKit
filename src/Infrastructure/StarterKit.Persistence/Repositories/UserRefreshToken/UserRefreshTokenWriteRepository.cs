using StarterKit.Application.Repositories.UserRefreshToken;
using StarterKit.Persistence.Contexts;

namespace StarterKit.Persistence.Repositories.UserRefreshToken
{
    public class UserRefreshTokenWriteRepository : WriteRepository<Domain.Entities.Identity.UserRefreshToken>, IUserRefreshTokenWriteRepository
    {
        public UserRefreshTokenWriteRepository(ApplicationDbContext context) : base(context)
        {
            
        }
    }
}
