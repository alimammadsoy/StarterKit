using MediatR;
using Microsoft.AspNetCore.Identity;
using StarterKit.Application.Abstractions.Services;
using StarterKit.Application.Exceptions;
using StarterKit.Application.Repositories.PasswordReset;
using System.Security.Cryptography;

namespace StarterKit.Application.Features.Command.AppUser.ForgotPassword
{
    public class ForgotPasswordCommandHandler : IRequestHandler<ForgotPasswordCommandRequest, ForgotPasswordCommandResponse>
    {
        private readonly UserManager<Domain.Entities.Identity.AppUser> _userManager;
        private readonly IPasswordResetReadRepository _passwordResetReadRepository;
        private readonly IPasswordResetWriteRepository _passwordResetWriteRepository;
        private readonly IMailService _mailService;

        public ForgotPasswordCommandHandler(
            UserManager<Domain.Entities.Identity.AppUser> userManager,
            IPasswordResetReadRepository passwordResetReadRepository,
            IPasswordResetWriteRepository passwordResetWriteRepository,
            IMailService mailService)
        {
            _userManager = userManager;
            _passwordResetReadRepository = passwordResetReadRepository;
            _passwordResetWriteRepository = passwordResetWriteRepository;
            _mailService = mailService;
        }

        public async Task<ForgotPasswordCommandResponse> Handle(ForgotPasswordCommandRequest request, CancellationToken cancellationToken)
        {
            DateTime expiresAt = DateTime.UtcNow.AddMinutes(5);

            // 1. User tapılır
            var user = await _userManager.FindByEmailAsync(request.Email);
            if (user == null)
                throw new NotFoundException("UserNotFound");

            // 2. Artıq mövcud aktiv OTP var?
            var existingOtp = await _passwordResetReadRepository
                .GetSingleAsync(x => x.UserId == user.Id);

            if (existingOtp != null)
            {
                // Eger kod hele de etibarlidirsa, yeniden gondermesini onleyirik
                if (existingOtp.ExpiresAt > DateTime.UtcNow)
                {
                    throw new CustomHttpException(409, new
                    {
                        error_code = 1002,
                        message = "VerificationCodeAlreadySent",
                        expires_at = existingOtp.ExpiresAt
                    });
                }

                // Eger kodun vaxti kecibse yenileyirik
                int newCode = GenerateVerificationCode();
                existingOtp.SetDetails(user.Id, newCode, expiresAt);

                _passwordResetWriteRepository.Update(existingOtp);
                await _passwordResetWriteRepository.SaveAsync();

                await SendVerificationMailAsync(request.Email, newCode);

                return new()
                {
                    Message = "VerificationCodeSent",
                    ExpiresAt = expiresAt
                };
            }

            // 3. Yeni password reset row yaradılır

            Domain.Entities.Identity.PasswordReset passwordReset = new();

            int code = GenerateVerificationCode();

            passwordReset.SetDetails(user.Id, code, expiresAt);

            await _passwordResetWriteRepository.AddAsync(passwordReset);
            await _passwordResetWriteRepository.SaveAsync();

            // 4. Email göndərilir
            await SendVerificationMailAsync(request.Email, code);

            return new()
            {
                Message = "VerificationCodeSent",
                ExpiresAt = expiresAt
            };
        }

        private async Task SendVerificationMailAsync(string email, int code)
        {
            string subject = "Email Təsdiqləmə Kodu";
            string body = $"Şifrəni yeniləmək üçün aşağıdakı 6 rəqəmli kodu istifadə edin: {code}";

            await _mailService.SendMailAsync(email, subject, body);
        }

        private static int GenerateVerificationCode()
        {
            byte[] randomBytes = new byte[4];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(randomBytes);
            }

            int randomNumber = Math.Abs(BitConverter.ToInt32(randomBytes, 0));
            return (randomNumber % 900000) + 100000;
        }
    }
}
