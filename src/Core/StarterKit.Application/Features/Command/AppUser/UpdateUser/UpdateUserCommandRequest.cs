using MediatR;
using StarterKit.Application.Consts;

namespace StarterKit.Application.Features.Command.AppUser.UpdateUser
{
    public class UpdateUserCommandRequest : IRequest<ResponseDto>
    {
        public int Id { get; set; }
        public string NameSurname { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public int[] RoleIds { get; set; }
    }
}
