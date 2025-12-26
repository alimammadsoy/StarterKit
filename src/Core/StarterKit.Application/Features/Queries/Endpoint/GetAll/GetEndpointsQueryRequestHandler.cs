using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using StarterKit.Application.Consts;
using StarterKit.Application.DTOs.Endpoint;
using StarterKit.Application.Exceptions;
using StarterKit.Application.Repositories.Endpoint;

namespace StarterKit.Application.Features.Queries.Endpoint.GetAll
{
    public class GetEndpointsQueryHandler : IRequestHandler<GetEndpointsQueryRequest, AllDto<EndpointDto>>
    {
        private readonly IEndpointReadRepository _endpointReadRepository;
        private readonly IMapper _mapper;

        public GetEndpointsQueryHandler(IEndpointReadRepository endpointReadRepository, IMapper mapper)
        {
            _endpointReadRepository =endpointReadRepository;
            _mapper = mapper;
        }

        public async Task<AllDto<EndpointDto>> Handle(GetEndpointsQueryRequest request, CancellationToken cancellationToken)
        {
            var endpoints = _endpointReadRepository.GetWhere(e => !e.IsDeleted,
                include => include.Include(e => e.Menu).Include(e => e.Roles));

            if (!await endpoints.AnyAsync(cancellationToken))
                throw new NotFoundException("İcazə tapılmadı");


            endpoints = endpoints.ApplySortingAndFiltering(request.ColumnName, request.OrderBy, request.Search);

            int totalCount = await endpoints.CountAsync();

            int totalPage = request.PageSize != null
                ? (int)Math.Ceiling(totalCount / (double)request.PageSize)
            : 1;

            if (request.PageNumber != null && request.PageSize != null)
            {
                endpoints = endpoints.Skip((request.PageNumber.Value - 1) * request.PageSize.Value).Take(request.PageSize.Value);
            }

            List<EndpointDto> endpointsDto = _mapper
                .Map<List<EndpointDto>>(await endpoints.ToListAsync(cancellationToken));

            return new AllDto<EndpointDto>
            {
                Data = endpointsDto,
                TotalCount = totalCount,
                TotalPage = totalPage,
            };
        }
    }
}
