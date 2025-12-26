using MediatR;
using Microsoft.AspNetCore.Identity;
using StarterKit.Application.Consts;
using StarterKit.Application.Exceptions;
using StarterKit.Application.Repositories.PasswordReset;

namespace StarterKit.Application.Features.Command.AppUser.VerifyResetCode
{
    public class VerifyResetCodeCommandHandler : IRequestHandler<VerifyResetCodeCommandRequest, ResponseDto>
    {
        private readonly UserManager<Domain.Entities.Identity.AppUser> _userManager;
        private readonly IPasswordResetReadRepository _passwordResetReadRepository;

        public VerifyResetCodeCommandHandler(
            UserManager<Domain.Entities.Identity.AppUser> userManager,
            IPasswordResetReadRepository passwordResetReadRepository)
        {
            _userManager = userManager;
            _passwordResetReadRepository = passwordResetReadRepository;
        }

        public async Task<ResponseDto> Handle(VerifyResetCodeCommandRequest request, CancellationToken cancellationToken)
        {
            // 1. User tapılır
            var user = await _userManager.FindByEmailAsync(request.Email);
            if (user == null)
                throw new NotFoundException("UserNotFound");

            // 2. OTP tapılır
            var otpRecord = await _passwordResetReadRepository
                .GetSingleAsync(x => x.UserId == user.Id && !x.IsDeleted);

            // 2️. Kod düzgünlük yoxlaması
            if (otpRecord.Code != request.Otp)
                throw new BadRequestException("InvalidVerificationCode");

            // 3. Expire olub?
            if (otpRecord.ExpiresAt < DateTime.UtcNow)
                throw new BadRequestException("VerificationCodeExpired");

            return new ResponseDto
            {
                Message = "Confirmed",
            };
        }
    }

}
