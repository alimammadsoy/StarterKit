using MediatR;
using StarterKit.Application.DTOs.User;

namespace StarterKit.Application.Features.Queries.User.GetById
{
    public class GetUserByIdQueryRequest : IRequest<UserDto>
    {
        public int Id { get; set; }

        public GetUserByIdQueryRequest(int id)
        {
            Id = id;
        }
    }
}
