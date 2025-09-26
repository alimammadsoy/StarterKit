using MediatR;
using StarterKit.Application.Abstractions.Services;
using StarterKit.Application.Consts;

namespace StarterKit.Application.Features.Command.AppUser.PasswordReset
{
    public class PasswordResetCommandHandler : IRequestHandler<PasswordResetCommandRequest, ResponseDto>
    {
        private readonly IAuthService _authService;

        public PasswordResetCommandHandler(IAuthService authService)
        {
            _authService = authService;
        }

        public async Task<ResponseDto> Handle(PasswordResetCommandRequest request, CancellationToken cancellationToken)
        {
            await _authService.PasswordResetAsnyc(request.Email);
            return new()
            {
                Message = "Elektron poçtunuza şifrənizi dəyişdirməklə bağlı mail göndərildi."
            };
        }
    }
}
