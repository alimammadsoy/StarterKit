using StarterKit.Application.Consts;

namespace StarterKit.Application.Features.Command.AppUser.ForgotPassword
{
    public class ForgotPasswordCommandResponse :  ResponseDto
    {
        public DateTime ExpiresAt { get; set; }
    }
}
