using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using StarterKit.Application.DTOs.Role;
using StarterKit.Application.DTOs.User;
using StarterKit.Application.Exceptions;
using StarterKit.Application.Features.Queries.User.GetById;
using StarterKit.Domain.Entities.Identity;

namespace StarterKit.Application.Features.Queries.User.GetUserById
{
    public class GetUserByIdQueryRequestHandler : IRequestHandler<GetUserByIdQueryRequest, UserDto>
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly RoleManager<AppRole> _roleManager;

        public GetUserByIdQueryRequestHandler(UserManager<AppUser> userManager, RoleManager<AppRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public async Task<UserDto> Handle(GetUserByIdQueryRequest request, CancellationToken cancellationToken)
        {
            var user = await _userManager.Users.FirstOrDefaultAsync(u => u.Id == request.Id && !u.IsDeleted, cancellationToken);
            if (user == null)
                throw new NotFoundException("İstifadəçi tapılmadı");

            var roleNames = await _userManager.GetRolesAsync(user);

            var roleDtos = new List<RoleDto>();
            if (roleNames.Any())
            {
                var roles = await _roleManager.Roles
                    .Where(r => roleNames.Contains(r.Name))
                    .ToListAsync(cancellationToken);

                roleDtos = roles.Select(r => new RoleDto
                {
                    Id = r.Id,
                    Name = r.Name
                }).ToList();
            }

            return new()
            {
                Id = user.Id,
                Email = user.Email,
                Name = user.Name,
                Surname = user.Surname,
                Phone = user.PhoneNumber,
                Roles = roleDtos
            };
        }
    }
}
