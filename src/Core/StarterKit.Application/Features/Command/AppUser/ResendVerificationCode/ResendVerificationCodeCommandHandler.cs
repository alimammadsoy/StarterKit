using MediatR;
using StarterKit.Application.Abstractions.Services;
using StarterKit.Application.Exceptions;
using StarterKit.Application.Repositories.TemporaryUser;
using System.Security.Cryptography;

namespace StarterKit.Application.Features.Command.AppUser.ResendVerificationCode
{
    public class ResendVerificationCodeCommandHandler
        : IRequestHandler<ResendVerificationCodeCommandRequest, ResendVerificationCodeCommandResponse>
    {
        private readonly ITemporaryUserReadRepository _temporaryUserReadRepository;
        private readonly ITemporaryUserWriteRepository _temporaryUserWriteRepository;
        private readonly IMailService _mailService;


        public ResendVerificationCodeCommandHandler(
            ITemporaryUserReadRepository temporaryUserReadRepository,
            ITemporaryUserWriteRepository temporaryUserWriteRepository,
            IMailService mailService)
        {
            _temporaryUserReadRepository = temporaryUserReadRepository;
            _temporaryUserWriteRepository = temporaryUserWriteRepository;
            _mailService = mailService;
        }

        public async Task<ResendVerificationCodeCommandResponse> Handle(
            ResendVerificationCodeCommandRequest request,
            CancellationToken cancellationToken)
        {
            // 1️. Temporary user tapılır
            var tempUser = await _temporaryUserReadRepository
                .GetSingleAsync(x => x.Email == request.Email && !x.IsDeleted);

            if (tempUser == null)
                throw new BadRequestException("NotRegistered");

            // 2️. Əgər kod hələ etibarlıdırsa yeni kod göndərilməməlidir
            if (tempUser.ExpiresAt > DateTime.UtcNow)
                throw new BadRequestException("VerificationCodeAlreadySent");
            

            // 3. Yeni verification code yaradılır
            int newCode = GenerateVerificationCode();
            DateTime expiresAt = DateTime.UtcNow.AddMinutes(5);

            tempUser.SetVerificationCode(newCode, expiresAt);
            tempUser.IncrementResendCount();

            _temporaryUserWriteRepository.Update(tempUser);
            await _temporaryUserWriteRepository.SaveAsync();

            // 4. Kod email/SMS ilə göndərilir

            string newSubject = "Email Təsdiqləmə Kodu";
            string newBody = $"Qeydiyyatı tamamlamaq üçün aşağıdakı 6 rəqəmli kodu istifadə edin: {newCode}";

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
