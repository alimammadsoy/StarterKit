using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using StarterKit.Domain.Entities.Identity;
using StarterKit.Persistence.Contexts;

namespace StarterKit.Persistence.Services
{
    public class CustomUserStore : UserStore<AppUser, AppRole, ApplicationDbContext, int>
    {
        public CustomUserStore(ApplicationDbContext context, IdentityErrorDescriber describer = null)
            : base(context, describer) { }

        public override Task<AppUser> FindByNameAsync(string normalizedUserName, CancellationToken cancellationToken = default)
        {
            return Users.FirstOrDefaultAsync(u => u.NormalizedUserName == normalizedUserName && !u.IsDeleted, cancellationToken);
        }

        public override Task<AppUser> FindByEmailAsync(string normalizedEmail, CancellationToken cancellationToken = default)
        {
            return Users.FirstOrDefaultAsync(u => u.NormalizedEmail == normalizedEmail && !u.IsDeleted, cancellationToken);
        }
    }

}
