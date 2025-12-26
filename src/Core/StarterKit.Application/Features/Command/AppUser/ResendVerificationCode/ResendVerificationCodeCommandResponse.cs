using StarterKit.Application.Consts;

namespace StarterKit.Application.Features.Command.AppUser.ResendVerificationCode
{
    public class ResendVerificationCodeCommandResponse : ResponseDto
    {
        public DateTime ExpiresAt { get; set; }
    }
}