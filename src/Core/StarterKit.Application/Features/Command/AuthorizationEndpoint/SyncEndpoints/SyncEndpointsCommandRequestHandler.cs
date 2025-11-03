using MediatR;
using StarterKit.Application.Abstractions.Services;
using StarterKit.Application.Consts;

namespace StarterKit.Application.Features.Command.AuthorizationEndpoint.SyncEndpoints
{
    public class SyncEndpointsCommandHandler : IRequestHandler<SyncEndpointsCommandRequest, ResponseDto>
    {
        readonly IAuthorizationEndpointService _authorizationEndpointService;

        public SyncEndpointsCommandHandler(IAuthorizationEndpointService authorizationEndpointService)
        {
            _authorizationEndpointService = authorizationEndpointService;
        }

        public async Task<ResponseDto> Handle(SyncEndpointsCommandRequest request, CancellationToken cancellationToken)
        {
            await _authorizationEndpointService.SyncEndpointsAsync();
            return new ResponseDto
            {
                Message = "Endpoints synchronized successfully"
            };
        }
    }
}
