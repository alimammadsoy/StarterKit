using MediatR;
using StarterKit.Application.Consts;

namespace StarterKit.Application.Features.Command.AppUser.ResetPassword
{
    public class ResetPasswordCommandRequest : IRequest<ResponseDto>
    {
        public string Email { get; set; }
        public int Otp { get; set; } //OTP
        public string NewPassword { get; set; }
        public string NewPasswordConfirmation { get; set; }
    }
}