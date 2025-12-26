using MediatR;
using StarterKit.Application.Consts;

namespace StarterKit.Application.Features.Command.AppUser.ForgotPassword
{
    public class ForgotPasswordCommandRequest : IRequest<ForgotPasswordCommandResponse>
    {
        public string Email { get; set; }
    }
}