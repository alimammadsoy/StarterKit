using MediatR;
using StarterKit.Application.Consts;

namespace StarterKit.Application.Features.Command.AppUser.LogoutUser
{
    public class LogoutCommandRequest : IRequest<ResponseDto>
    {
        public string RefreshToken { get; set; }
    }
}
