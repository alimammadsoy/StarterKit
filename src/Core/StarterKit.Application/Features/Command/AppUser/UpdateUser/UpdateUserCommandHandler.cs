using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using StarterKit.Application.Abstractions.Services;
using StarterKit.Application.Consts;
using StarterKit.Application.Exceptions;
using StarterKit.Domain.Entities.Identity;

namespace StarterKit.Application.Features.Command.AppUser.UpdateUser
{
    public class UpdateUserCommandHandler : IRequestHandler<UpdateUserCommandRequest, ResponseDto>
    {
        private readonly UserManager<Domain.Entities.Identity.AppUser> _userManager;
        private readonly RoleManager<AppRole> _roleManager;
        private readonly IUserService _userService;

        public UpdateUserCommandHandler(UserManager<Domain.Entities.Identity.AppUser> userManager,
                                        RoleManager<AppRole> roleManager,
                                        IUserService userService)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _userService = userService;
        }

        public async Task<ResponseDto> Handle(UpdateUserCommandRequest request, CancellationToken cancellationToken)
        {
            var user = await _userManager.FindByIdAsync(request.Id.ToString());
            if (user == null)
                throw new NotFoundException("İstifadəçi tapılmadı");

            if (!string.IsNullOrWhiteSpace(request.NameSurname))
                user.NameSurname = request.NameSurname;

            if (!string.IsNullOrWhiteSpace(request.Username) && user.UserName != request.Username)
            {
                user.UserName = request.Username;
                user.NormalizedUserName = request.Username.ToUpperInvariant();
            }

            if (!string.IsNullOrWhiteSpace(request.Email) && user.Email != request.Email)
            {
                user.Email = request.Email;
                user.NormalizedEmail = request.Email.ToUpperInvariant();
            }

            var updateResult = await _userManager.UpdateAsync(user);
            if (!updateResult.Succeeded)
            {
                var errors = string.Join("; ", updateResult.Errors.Select(e => e.Description));
                return new ResponseDto { Message = $"Failed to update user: {errors}" };
            }

            if (request.RoleIds != null)
            {
                var roleNames = await _roleManager.Roles
                    .Where(r => request.RoleIds.Contains(r.Id))
                    .Select(r => r.Name)
                    .ToListAsync(cancellationToken);

                await _userService.AssignRoleToUserAsnyc(request.Id, roleNames.ToArray());
            }

            return new ResponseDto { Message = "İstifadəçi uğurla redaktə edildi" };
        }
    }
}