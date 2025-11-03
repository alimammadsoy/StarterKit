using MediatR;
using StarterKit.Application.Consts;
using StarterKit.Application.DTOs.Endpoint;

namespace StarterKit.Application.Features.Queries.Endpoint.GetAll
{
    public class GetEndpointsQueryRequest : IRequest<AllDto<EndpointDto>>
    {
        public int? PageNumber { get; set; }
        public int? PageSize { get; set; }
    }
}
