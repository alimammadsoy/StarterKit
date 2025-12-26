using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using StarterKit.Application.Abstractions.Services;
using StarterKit.Application.Consts;
using StarterKit.Application.DTOs.Role;

namespace StarterKit.Application.Features.Queries.Role.GetRoles
{
    public class GetRolesQueryHandler : IRequestHandler<GetRolesQueryRequest, AllDto<RoleDto>>
    {
        readonly IRoleService _roleService;
        private readonly IMapper _mapper;

        public GetRolesQueryHandler(IRoleService roleService, IMapper mapper)
        {
            _roleService = roleService;
            _mapper = mapper;
        }

        public async Task<AllDto<RoleDto>> Handle(GetRolesQueryRequest request, CancellationToken cancellationToken)
        {
            var roles = _roleService.GetAllRolesAsync();

            roles = roles.ApplySortingAndFiltering(request.ColumnName, request.OrderBy, request.Search);

            var totalCount = await roles.CountAsync();

            if (request.PageNumber != null && request.PageSize != null)
            {
                roles = roles.Skip((request.PageNumber.Value - 1) * request.PageSize.Value).Take(request.PageSize.Value);
            }

            var totalPage = request.PageNumber != null
                ? (int)Math.Ceiling(totalCount / (double)request.PageSize)
                : 1;

            List<RoleDto> roleDtos = _mapper.Map<List<RoleDto>>(await roles.ToListAsync(cancellationToken));


            return new AllDto<RoleDto>
            {
                Data = roleDtos,
                TotalCount = totalCount,
                TotalPage = totalPage
            };
        }
    }
}
