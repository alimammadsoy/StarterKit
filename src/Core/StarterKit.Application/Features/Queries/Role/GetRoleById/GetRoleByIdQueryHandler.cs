using MediatR;
using StarterKit.Application.Abstractions.Services;
using StarterKit.Application.DTOs.Endpoint;
using StarterKit.Application.Exceptions;

namespace StarterKit.Application.Features.Queries.Role.GetRoleById
{
    public class GetRoleByIdQueryHandler : IRequestHandler<GetRoleByIdQueryRequest, GetRoleByIdQueryResponse>
    {
        readonly IRoleService _roleService;

        public GetRoleByIdQueryHandler(IRoleService roleService)
        {
            _roleService = roleService;
        }

        public async Task<GetRoleByIdQueryResponse> Handle(GetRoleByIdQueryRequest request, CancellationToken cancellationToken)
        {
            var role = await _roleService.GetRoleById(request.Id);

            if (role is null)
                throw new NotFoundException("Rol tapılmadı");


            return new()
            {
                Id = role.Id,
                Name = role.Name,
                Permissions = role.Endpoints?
                    .Select(e => new EndpointDto
                    {
                        Id = e.Id,
                        Name = e.Definition
                    }).ToList() ?? new List<EndpointDto>()
            };
        }
    }
}
