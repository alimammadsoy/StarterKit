using MediatR;

namespace StarterKit.Application.Features.Queries.User.GetRolesToUser
{
    public class GetRolesToUserQueryRequest : IRequest<GetRolesToUserQueryResponse>
    {
        public int UserId { get; set; }
    }
}