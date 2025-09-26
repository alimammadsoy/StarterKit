using MediatR;
using StarterKit.Application.Abstractions.Services;
using StarterKit.Application.Consts;

namespace StarterKit.Application.Features.Command.AuthorizationEndpoint.AssignRoleEndpoint
{
    public class AssignRoleEndpointCommandHandler : IRequestHandler<AssignRoleEndpointCommandRequest, ResponseDto>
    {
        readonly IAuthorizationEndpointService _authorizationEndpointService;

        public AssignRoleEndpointCommandHandler(IAuthorizationEndpointService authorizationEndpointService)
        {
            _authorizationEndpointService = authorizationEndpointService;
        }

        public async Task<ResponseDto> Handle(AssignRoleEndpointCommandRequest request, CancellationToken cancellationToken)
        {
            await _authorizationEndpointService.AssignRoleEndpointAsync(request.Roles, request.Menu, request.Code, request.Type);
            return new()
            {
                Message = "Rollar uğurla əlaqələndirildi."
            };
        }
    }
}
