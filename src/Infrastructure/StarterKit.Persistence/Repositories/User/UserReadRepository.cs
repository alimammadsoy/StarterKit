using Microsoft.EntityFrameworkCore;
using StarterKit.Application.Repositories.User;
using StarterKit.Persistence.Contexts;

namespace StarterKit.Persistence.Repositories.User
{
    public class UserReadRepository : IUserReadRepository
    {
        private readonly ApplicationDbContext _context;

        public UserReadRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<bool> ExistsActiveUserByEmailAsync(string email)
        {
            return await _context.Users
                .AnyAsync(x => x.Email == email && !x.IsDeleted);
        }

        public async Task<bool> ExistsActiveUserByUsernameAsync(string username)
        {
            return await _context.Users
                .AnyAsync(x => x.UserName == username && !x.IsDeleted);
        }
    }

}
