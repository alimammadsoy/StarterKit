using MediatR;
using StarterKit.Application.Features.Command.AppUser.DTOs;

namespace StarterKit.Application.Features.Command.AppUser.LoginUser
{
    public class LoginUserCommandRequest : IRequest<JwtTokenDto>
    {
        public string UsernameOrEmail { get; set; }
        public string Password { get; set; }
    }
}
