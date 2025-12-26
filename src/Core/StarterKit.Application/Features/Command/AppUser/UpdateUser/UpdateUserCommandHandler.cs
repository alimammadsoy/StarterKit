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

            if (!string.IsNullOrWhiteSpace(request.Name))
                user.Name = request.Name;

            if (!string.IsNullOrWhiteSpace(request.Surname))
                user.Name = request.Surname;

            if (!string.IsNullOrWhiteSpace(request.Phone) && user.PhoneNumber != request.Phone)
            {
                user.PhoneNumber = request.Phone;
            }

            if (!string.IsNullOrWhiteSpace(request.Email) && user.Email != request.Email)
            {
                var existingUser = await _userManager.FindByEmailAsync(request.Email);
                if (existingUser != null && existingUser.Id != user.Id)
                    throw new UserAlreadyExistedException("UserAlreadyActivated");

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

            return new ResponseDto { Message = "UserUpdatedSuccessfully" };
        }
    }
}