using MediatR;
using Microsoft.AspNetCore.Identity;
using StarterKit.Application.Abstractions.Services;
using StarterKit.Application.Consts;
using StarterKit.Domain.Entities.Identity;

namespace StarterKit.Application.Features.Commands.Role.CreateRole
{
    public class CreateRoleCommandHandler : IRequestHandler<CreateRoleCommandRequest, ResponseDto>
    {
        readonly IRoleService _roleService;
        private readonly IAuthorizationEndpointService _authorizationEndpointService;
        private readonly RoleManager<AppRole> _roleManager;

        public CreateRoleCommandHandler(IRoleService roleService, IAuthorizationEndpointService authorizationEndpointService,
            RoleManager<AppRole> roleManager)
        {
            _roleService = roleService;
            _authorizationEndpointService = authorizationEndpointService;
            _roleManager = roleManager;
        }

        public async Task<ResponseDto> Handle(CreateRoleCommandRequest request, CancellationToken cancellationToken)
        {
            var result = await _roleService.CreateRole(request.Name);

            if (result == true && request.PermissionIds != null)
            {
                var role = await _roleManager.FindByNameAsync(request.Name);

                var permissionIds = request.PermissionIds
                    .Where(x => x.HasValue)
                    .Select(x => x.Value)
                    .ToArray();

                if (permissionIds.Length > 0)
                    await _authorizationEndpointService.AssignEndpointsToRoleAsync(role.Id, permissionIds);
            }

            return new()
            {
                Message = "Rol əlavə olundu"
            };
        }
    }
}
