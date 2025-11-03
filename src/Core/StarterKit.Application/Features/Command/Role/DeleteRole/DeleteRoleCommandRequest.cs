using MediatR;
using StarterKit.Application.Consts;

namespace StarterKit.Application.Features.Commands.Role.DeleteRole
{
    public class DeleteRoleCommandRequest : IRequest<ResponseDto>
    {
        public int Id { get; set; }
    }
}