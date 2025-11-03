using MediatR;

namespace StarterKit.Application.Features.Queries.Role.GetRoleById
{
    public class GetRoleByIdQueryRequest : IRequest<GetRoleByIdQueryResponse>
    {
        public int Id { get; set; }
    }
}