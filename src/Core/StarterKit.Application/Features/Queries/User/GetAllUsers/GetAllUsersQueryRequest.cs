using MediatR;

namespace StarterKit.Application.Features.Queries.User.GetAllUsers
{
    public class GetAllUsersQueryRequest : IRequest<GetAllUsersQueryResponse>
    {
        public int? PageNumber { get; set; }
        public int? PageSize { get; set; }
    }
}