using MediatR;
using StarterKit.Application.DTOs.Auth;

namespace StarterKit.Application.Features.Command.AppUser.RefreshToken
{
    public class RefreshTokenLoginCommandRequest : IRequest<JwtTokenDto>
    {
        public string RefreshToken { get; set; }
    }
}
