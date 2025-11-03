using MediatR;
using StarterKit.Application.Consts;

namespace StarterKit.Application.Features.Commands.Role.CreateRole
{
    public class CreateRoleCommandRequest : IRequest<ResponseDto>
    {
        public string Name { get; set; }
    }
}