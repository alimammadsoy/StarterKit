using AutoMapper;
using MediatR;
using StarterKit.Application.DTOs.Endpoint;
using StarterKit.Application.Exceptions;
using StarterKit.Application.Repositories.Endpoint;

namespace StarterKit.Application.Features.Queries.Endpoint.GetById
{
    public class GetEndpointByIdQueryRequestHandler : IRequestHandler<GetEndpointByIdQueryRequest, EndpointDto>
    {
        private readonly IEndpointReadRepository _endpointReadRepository;
        private readonly IMapper _mapper;

        public GetEndpointByIdQueryRequestHandler(IEndpointReadRepository endpointReadRepository, IMapper mapper)
        {
            _endpointReadRepository = endpointReadRepository;
            _mapper = mapper;
        }

        public async Task<EndpointDto> Handle(GetEndpointByIdQueryRequest request, CancellationToken cancellationToken)
        {
            var endpoint = await _endpointReadRepository.GetByIdAsync(request.Id);

            if (endpoint == null)
                throw new NotFoundException("İcazə tapılmadı");

            return _mapper.Map<EndpointDto>(endpoint);
        }
    }
}
