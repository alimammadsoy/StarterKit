using MediatR;
using Microsoft.Extensions.Configuration;
using StarterKit.Application.Abstractions.Services;
using StarterKit.Application.DTOs.Auth;

namespace StarterKit.Application.Features.Command.AppUser.LoginUser
{
    public class LoginUserCommandHandler : IRequestHandler<LoginUserCommandRequest, JwtTokenDto>
    {
        readonly IAuthService _authService;
        private readonly IConfiguration _configuration;

        public LoginUserCommandHandler(IAuthService authService, IConfiguration configuration)
        {
            _authService = authService;
            _configuration = configuration;
        }

        public async Task<JwtTokenDto> Handle(LoginUserCommandRequest request, CancellationToken cancellationToken)
        {
            var expireAt = Convert.ToInt32(_configuration["JWT:ExpireAt"]);
            var token = await _authService.LoginAsync(request.Email, request.Password, expireAt);

            return new() { AccessToken = token.AccessToken, RefreshToken = token.RefreshToken, ExpiresAt = token.ExpiresAt };
        }
    }
}
