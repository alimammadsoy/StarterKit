using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using StarterKit.Application.Abstractions.Services;
using StarterKit.Application.Consts;
using StarterKit.Domain.Entities.Identity;

namespace StarterKit.Application.Features.Command.AppUser.CreateUser
{
    public class CreateUserCommandHandler : IRequestHandler<CreateUserCommandRequest, ResponseDto>
    {
        readonly IUserService _userService;
        readonly UserManager<Domain.Entities.Identity.AppUser> _userManager;
        readonly RoleManager<AppRole> _roleManager;

        public CreateUserCommandHandler(IUserService userService, UserManager<Domain.Entities.Identity.AppUser> userManager, RoleManager<AppRole> roleManager)
        {
            _userService = userService;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public async Task<ResponseDto> Handle(CreateUserCommandRequest request, CancellationToken cancellationToken)
        {
            var createResponse = await _userService.CreateAsync(new()
            {
                Email = request.Email,
                NameSurname = request.NameSurname,
                Password = request.Password,
                PasswordConfirm = request.PasswordConfirm,
                Username = request.Username,
            });

            if (createResponse?.Succeeded == true && request.RoleIds != null && request.RoleIds.Length > 0)
            {
                var user = await _userManager.FindByNameAsync(request.Username);
                if (user != null)
                {
                    var roleNames = await _roleManager.Roles
                        .Where(r => request.RoleIds.Contains(r.Id))
                        .Select(r => r.Name)
                        .ToListAsync(cancellationToken);

                    if (roleNames.Any())
                    {
                        await _userService.AssignRoleToUserAsnyc(user.Id, roleNames.ToArray());
                    }
                }
            }

            return new ResponseDto
            {
                Message = createResponse?.Message ?? "User creation failed."
            };
        }
    }
}
