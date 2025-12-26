using MediatR;
using StarterKit.Application.Consts;

namespace StarterKit.Application.Features.Command.AppUser.VerifyResetCode
{
    public class VerifyResetCodeCommandRequest : IRequest<ResponseDto>
    {
        public string Email { get; set; }
        public int Otp { get; set; }
    }
}