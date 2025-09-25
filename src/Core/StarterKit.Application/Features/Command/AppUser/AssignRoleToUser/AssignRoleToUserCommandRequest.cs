using MediatR;
using StarterKit.Application.Consts;

namespace StarterKit.Application.Features.Command.AppUser.AssignRoleToUser
{
    public class AssignRoleToUserCommandRequest : IRequest<ResponseDto>
    {
        public string UserId { get; set; }
        public string[] Roles { get; set; }
    }
}