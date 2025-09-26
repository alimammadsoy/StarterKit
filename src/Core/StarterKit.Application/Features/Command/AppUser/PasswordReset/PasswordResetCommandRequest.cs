using MediatR;
using StarterKit.Application.Consts;

namespace StarterKit.Application.Features.Command.AppUser.PasswordReset
{
    public class PasswordResetCommandRequest : IRequest<ResponseDto>
    {
        public string Email { get; set; }
    }
}
