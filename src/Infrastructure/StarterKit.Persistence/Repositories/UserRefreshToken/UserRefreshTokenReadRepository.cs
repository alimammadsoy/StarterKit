using StarterKit.Application.Repositories.UserRefreshToken;
using StarterKit.Persistence.Contexts;

namespace StarterKit.Persistence.Repositories.UserRefreshToken
{
    public class UserRefreshTokenReadRepository : ReadRepository<Domain.Entities.Identity.UserRefreshToken>, IUserRefreshTokenReadRepository
    {
        public UserRefreshTokenReadRepository(ApplicationDbContext context) : base(context)
        {
            
        }
    }
}