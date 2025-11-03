using MediatR;
using StarterKit.Application.Consts;

namespace StarterKit.Application.Features.Command.AppUser.DeleteUser
{
    public class DeleteUserCommandRequest : IRequest<ResponseDto>
    {
        public int Id { get; set; }

        public DeleteUserCommandRequest(int id)
        {
            Id = id;
        }
    }
}
