using Microsoft.AspNetCore.Identity;
using StarterKit.Application.Repositories.User;

namespace StarterKit.Application.Features.Command.AppUser.CreateUser
{
    public class CustomUserValidator : IUserValidator<Domain.Entities.Identity.AppUser>
    {
        private readonly IUserStore<Domain.Entities.Identity.AppUser> _userStore;

        public CustomUserValidator(IUserStore<Domain.Entities.Identity.AppUser> userStore)
        {
            _userStore = userStore;
        }

        public async Task<IdentityResult> ValidateAsync(UserManager<Domain.Entities.Identity.AppUser> manager, Domain.Entities.Identity.AppUser user)
        {
            var errors = new List<IdentityError>();

            // Email
            var existingByEmail = await manager.FindByEmailAsync(user.Email);
            if (existingByEmail != null && existingByEmail.Id != user.Id)
            {
                errors.Add(new IdentityError
                {
                    Code = "DuplicateEmail",
                    Description = $"Email '{user.Email}' is already taken."
                });
            }

            // Username
            var existingByName = await manager.FindByNameAsync(user.UserName);
            if (existingByName != null && existingByName.Id != user.Id)
            {
                errors.Add(new IdentityError
                {
                    Code = "DuplicateUserName",
                    Description = $"Username '{user.UserName}' is already taken."
                });
            }

            return errors.Any() ? IdentityResult.Failed(errors.ToArray()) : IdentityResult.Success;
        }
    }

}
