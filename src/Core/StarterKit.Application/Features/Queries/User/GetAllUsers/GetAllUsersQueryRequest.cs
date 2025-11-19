using MediatR;
using StarterKit.Application.Consts;
using StarterKit.Application.DTOs.User;

namespace StarterKit.Application.Features.Queries.User.GetAllUsers
{
    public class GetAllUsersQueryRequest : IRequest<AllDto<ListUser>>
    {
        public int? PageNumber { get; set; }
        public int? PageSize { get; set; }
        public string? Search { get; set; }
        public string? ColumnName { get; set; }
        public string? OrderBy { get; set; }
    }
}