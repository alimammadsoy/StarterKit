using MediatR;
using StarterKit.Application.Abstractions.Services;
using StarterKit.Application.DTOs.Auth;

namespace StarterKit.Application.Features.Command.AppUser.RefreshToken
{
    public class RefreshTokenLoginCommandHandler : IRequestHandler<RefreshTokenLoginCommandRequest, JwtTokenDto>
    {
        private readonly IAuthService _authService;

        public RefreshTokenLoginCommandHandler(IAuthService authService)
        {
            _authService = authService;
        }

        public async Task<JwtTokenDto> Handle(RefreshTokenLoginCommandRequest request, CancellationToken cancellationToken)
        {
            JwtTokenDto token = await _authService.RefreshTokenLoginAsync(request.RefreshToken);
            return token;
        }
    }
}
