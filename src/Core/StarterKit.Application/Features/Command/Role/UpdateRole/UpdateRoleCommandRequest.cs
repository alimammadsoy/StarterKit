using MediatR;
using StarterKit.Application.Consts;

namespace StarterKit.Application.Features.Commands.Role.UpdateRole
{
    public class UpdateRoleCommandRequest : IRequest<ResponseDto>
    {
        public int Id { get; set; }       
        public string Name { get; set; }
        public int[] PermissionIds { get; set; }
    }
}