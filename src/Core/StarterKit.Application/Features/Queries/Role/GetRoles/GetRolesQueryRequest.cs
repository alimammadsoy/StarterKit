using MediatR;
using StarterKit.Application.Consts;
using StarterKit.Application.DTOs.Role;

namespace StarterKit.Application.Features.Queries.Role.GetRoles
{
    public class GetRolesQueryRequest : IRequest<AllDto<RoleDto>>
    {
        public int? PageNumber { get; set; }
        public int? PageSize { get; set; }
    }
}