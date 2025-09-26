using MediatR;
using StarterKit.Application.Abstractions.Services;
using StarterKit.Application.DTOs.Auth;

namespace StarterKit.Application.Features.Command.AppUser.LoginUser
{
    public class LoginUserCommandHandler : IRequestHandler<LoginUserCommandRequest, JwtTokenDto>
    {
        readonly IAuthService _authService;
        public LoginUserCommandHandler(IAuthService authService)
        {
            _authService = authService;
        }

        public async Task<JwtTokenDto> Handle(LoginUserCommandRequest request, CancellationToken cancellationToken)
        {
            var token = await _authService.LoginAsync(request.UsernameOrEmail, request.Password, 7200);

            return new() { AccessToken = token.AccessToken, RefreshToken = token.RefreshToken, Expiration = token.Expiration };
        }
    }
}
