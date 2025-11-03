using MediatR;
using Microsoft.AspNetCore.Identity;
using StarterKit.Application.Consts;
using StarterKit.Application.Abstractions.Services;
using StarterKit.Domain.Entities.Identity;
using Microsoft.EntityFrameworkCore;
using StarterKit.Application.Exceptions;

namespace StarterKit.Application.Features.Commands.Role.UpdateRole
{
    public class UpdateRoleCommandHandler : IRequestHandler<UpdateRoleCommandRequest, ResponseDto>
    {
        private readonly RoleManager<AppRole> _roleManager;
        private readonly IAuthorizationEndpointService _authorizationEndpointService;

        public UpdateRoleCommandHandler(RoleManager<AppRole> roleManager, IAuthorizationEndpointService authorizationEndpointService)
        {
            _roleManager = roleManager;
            _authorizationEndpointService = authorizationEndpointService;
        }

        public async Task<ResponseDto> Handle(UpdateRoleCommandRequest request, CancellationToken cancellationToken)
        {
            var role = await _roleManager.Roles.FirstOrDefaultAsync(r => r.Id == request.Id, cancellationToken);
            if (role == null)
                throw new NotFoundException("Rol tapılmadı");

            role.Name = request.Name;
            role.NormalizedName = request.Name?.ToUpperInvariant();

            var updateResult = await _roleManager.UpdateAsync(role);
            if (!updateResult.Succeeded)
            {
                var errors = string.Join("; ", updateResult.Errors.Select(e => e.Description));
                return new ResponseDto { Message = $"Failed to update role: {errors}" };
            }
            
            if (request.PermissionIds != null)
            {
                await _authorizationEndpointService.AssignEndpointsToRoleAsync(request.Id, request.PermissionIds);
            }

            return new ResponseDto { Message = "Rol uğurla redaktə edildi." };
        }
    }
}
