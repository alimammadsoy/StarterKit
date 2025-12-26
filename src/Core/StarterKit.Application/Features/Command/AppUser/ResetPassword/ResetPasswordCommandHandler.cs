using MediatR;
using Microsoft.AspNetCore.Identity;
using StarterKit.Application.Consts;
using StarterKit.Application.Exceptions;
using StarterKit.Application.Repositories.PasswordReset;

namespace StarterKit.Application.Features.Command.AppUser.ResetPassword
{
    public class ResetPasswordCommandHandler : IRequestHandler<ResetPasswordCommandRequest, ResponseDto>
    {
        private readonly UserManager<Domain.Entities.Identity.AppUser> _userManager;
        private readonly IPasswordResetReadRepository _passwordResetReadRepository;
        private readonly IPasswordResetWriteRepository _passwordResetWriteRepository;

        public ResetPasswordCommandHandler(
            UserManager<Domain.Entities.Identity.AppUser> userManager,
            IPasswordResetReadRepository passwordResetReadRepository,
            IPasswordResetWriteRepository passwordResetWriteRepository)
        {
            _userManager = userManager;
            _passwordResetReadRepository = passwordResetReadRepository;
            _passwordResetWriteRepository = passwordResetWriteRepository;
        }

        public async Task<ResponseDto> Handle(ResetPasswordCommandRequest request, CancellationToken cancellationToken)
        {
            // Email ilə user tapılır
            var user = await _userManager.FindByEmailAsync(request.Email);
            if (user == null)
                throw new NotFoundException("UserNotFound");

            // PasswordResets cədvəlindən OTP yoxlanır
            var resetRecord = await _passwordResetReadRepository
                .GetSingleAsync(x => x.UserId == user.Id && !x.IsDeleted);

            if (resetRecord == null)
                throw new NotFoundException("UserNotFound");

            if (resetRecord.Code != request.Otp)
                throw new BadRequestException("InvalidVerificationCode");

            if (resetRecord.ExpiresAt < DateTime.UtcNow)
                throw new BadRequestException("VerificationCodeExpired");

            // Password reset edilir
            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            var result = await _userManager.ResetPasswordAsync(user, token, request.NewPassword);

            if (!result.Succeeded)
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                throw new BadRequestException($"Password reset failed: {errors}");
            }
            // OTP artıq istifadə olunub deyə silə bilərik
            _passwordResetWriteRepository.Remove(resetRecord);
            await _passwordResetWriteRepository.SaveAsync();

            return new()
            {
                Message = "PasswordResetSuccessfully",
            };
        }
    }

}
