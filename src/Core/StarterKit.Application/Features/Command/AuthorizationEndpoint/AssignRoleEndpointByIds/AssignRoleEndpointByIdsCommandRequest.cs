using MediatR;
using StarterKit.Application.Consts;

namespace StarterKit.Application.Features.Command.AuthorizationEndpoint.AssignRoleEndpointByIds
{
    public class AssignRoleEndpointByIdsCommandRequest : IRequest<ResponseDto>
    {
        public int[] RoleIds { get; set; }
        public int EndpointId { get; set; }
    }
}