using MediatR;
using StarterKit.Application.Consts;

namespace StarterKit.Application.Features.Command.AppUser.ResendVerificationCode
{
    public class ResendVerificationCodeCommandRequest : IRequest<ResponseDto>
    {
        public string Email { get; set; }
    }
}