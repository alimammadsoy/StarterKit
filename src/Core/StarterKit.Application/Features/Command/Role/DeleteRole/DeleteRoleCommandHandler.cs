using MediatR;
using StarterKit.Application.Abstractions.Services;
using StarterKit.Application.Consts;

namespace StarterKit.Application.Features.Commands.Role.DeleteRole
{
    public class DeleteRoleCommandHandler : IRequestHandler<DeleteRoleCommandRequest, ResponseDto>
    {
        readonly IRoleService _roleService;

        public DeleteRoleCommandHandler(IRoleService roleService)
        {
            _roleService = roleService;
        }

        public async Task<ResponseDto> Handle(DeleteRoleCommandRequest request, CancellationToken cancellationToken)
        {
            var result = await _roleService.DeleteRole(request.Id);
            return new()
            {
                Message = "Rol uğurla silindi"
            };
        }
    }
}
