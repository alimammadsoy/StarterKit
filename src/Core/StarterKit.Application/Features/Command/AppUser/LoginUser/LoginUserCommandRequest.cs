using MediatR;
using StarterKit.Application.DTOs.Auth;

namespace StarterKit.Application.Features.Command.AppUser.LoginUser
{
    public class LoginUserCommandRequest : IRequest<JwtTokenDto>
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }
}