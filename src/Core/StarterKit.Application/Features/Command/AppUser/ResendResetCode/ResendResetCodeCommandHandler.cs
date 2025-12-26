using MediatR;
using Microsoft.AspNetCore.Identity;
using StarterKit.Application.Abstractions.Services;
using StarterKit.Application.Exceptions;
using StarterKit.Application.Repositories.PasswordReset;
using System.Security.Cryptography;

namespace StarterKit.Application.Features.Command.AppUser.ResendResetCode
{
    public class ResendResetCodeCommandHandler : IRequestHandler<ResendResetCodeCommandRequest, ResendResetCodeCommandResponse>
    {
        private readonly UserManager<Domain.Entities.Identity.AppUser> _userManager;
        private readonly IPasswordResetReadRepository _passwordResetReadRepository;
        private readonly IPasswordResetWriteRepository _passwordResetWriteRepository;
        private readonly IMailService _mailService;

        public ResendResetCodeCommandHandler(
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

        public async Task<ResendResetCodeCommandResponse> Handle(ResendResetCodeCommandRequest request, CancellationToken cancellationToken)
        {
            var user = await _userManager.FindByEmailAsync(request.Email);

            if (user == null)
                throw new NotFoundException("UserNotFound");

            // 1. user üçün ən son kodu tapırıq
            var passwordReset = await _passwordResetReadRepository
                .GetSingleAsync(x => x.UserId == user.Id && !x.IsDeleted);

            // 2️. Əgər kod hələ etibarlıdırsa yeni kod göndərilməməlidir
            if (passwordReset != null && passwordReset.ExpiresAt > DateTime.UtcNow)
                throw new BadRequestException("VerificationCodeAlreadySent");

            // 3. Yeni kod generasiya edilir
            var newCode = GenerateVerificationCode();
            DateTime expiresAt = DateTime.UtcNow.AddMinutes(5);

            passwordReset.SetDetails(user.Id, newCode, expiresAt);

            _passwordResetWriteRepository.Update(passwordReset);
            await _passwordResetWriteRepository.SaveAsync();


            // 4. Kod email/SMS ilə göndərilir

            string newSubject = "Email Təsdiqləmə Kodu";
            string newBody = $"Şifrəni yeniləmək üçün aşağıdakı 6 rəqəmli kodu istifadə edin: {newCode}";

            await _mailService.SendMailAsync(request.Email, newSubject, newBody);

            // 5. Mesaj qaytarılır
            return new()
            {
                Message = "VerificationCodeResent",
                ExpiresAt = expiresAt
            };
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
