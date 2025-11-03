using MediatR;
using StarterKit.Application.DTOs.Endpoint;

namespace StarterKit.Application.Features.Queries.Endpoint.GetById
{
    public class GetEndpointByIdQueryRequest : IRequest<EndpointDto>
    {
        public int Id { get; set; }
        public GetEndpointByIdQueryRequest(int id)
        {
            Id = id;
        }
    }
}
