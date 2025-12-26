using StarterKit.Application.Consts;

namespace StarterKit.Application.Features.Command.AppUser.RegisterUser
{
    public class RegisterUserCommandResponse : ResponseDto
    {
        public DateTime ExpiresAt { get; set; }
    }
}
