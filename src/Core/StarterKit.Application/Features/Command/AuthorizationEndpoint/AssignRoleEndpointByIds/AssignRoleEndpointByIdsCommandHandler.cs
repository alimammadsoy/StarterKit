using MediatR;
using StarterKit.Application.Abstractions.Services;
using StarterKit.Application.Consts;

namespace StarterKit.Application.Features.Command.AuthorizationEndpoint.AssignRoleEndpointByIds
{
    public class AssignRoleEndpointByIdsCommandHandler : IRequestHandler<AssignRoleEndpointByIdsCommandRequest, ResponseDto>
    {
        readonly IAuthorizationEndpointService _authorizationEndpointService;

        public AssignRoleEndpointByIdsCommandHandler(IAuthorizationEndpointService authorizationEndpointService)
        {
            _authorizationEndpointService = authorizationEndpointService;
        }

        public async Task<ResponseDto> Handle(AssignRoleEndpointByIdsCommandRequest request, CancellationToken cancellationToken)
        {
            //await _authorizationEndpointService.AssignRoleEndpointByIdsAsync(request.RoleIds, request.EndpointId);
            return new ResponseDto
            {
                Message = "İcazə uğurla əlaqələndirildi."
            };
        }
    }
}