using StarterKit.Application.Consts;

namespace StarterKit.Application.Features.Command.AppUser.ResendResetCode
{
    public class ResendResetCodeCommandResponse : ResponseDto
    {
        public DateTime ExpiresAt { get; set; }
    }
}
