using MediatR;

namespace StarterKit.Application.Features.Command.AppUser.ResendResetCode
{
    public class ResendResetCodeCommandRequest : IRequest<ResendResetCodeCommandResponse>
    {
        public string Email { get; set; }
    }
}