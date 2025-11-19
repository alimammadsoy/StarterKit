using MediatR;
using StarterKit.Application.Consts;

namespace StarterKit.Application.Features.Command.AppUser.CreateUser
{
    public class CreateUserCommandRequest : IRequest<ResponseDto>
    {
        public string Name { get; set; }
        public string Surname { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Password { get; set; }
        public string PasswordConfirmation { get; set; }
        public int[] RoleIds { get; set; }
    }
}