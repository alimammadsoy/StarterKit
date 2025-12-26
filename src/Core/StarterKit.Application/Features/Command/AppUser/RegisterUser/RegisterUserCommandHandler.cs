using MediatR;
using Microsoft.AspNetCore.Identity;
using StarterKit.Application.Abstractions.Services;
using StarterKit.Application.Exceptions;
using StarterKit.Application.Repositories.TemporaryUser;
using StarterKit.Domain.Entities.Identity;
using System.Security.Cryptography;

namespace StarterKit.Application.Features.Command.AppUser.RegisterUser
{
    public class RegisterUserCommandHandler : IRequestHandler<RegisterUserCommandRequest, RegisterUserCommandResponse>
    {
        private readonly ITemporaryUserWriteRepository _temporaryUserWriteRepository;
        private readonly ITemporaryUserReadRepository _temporaryUserReadRepository;
        private readonly UserManager<Domain.Entities.Identity.AppUser> _userManager;
        private readonly IMailService _mailService;


        public RegisterUserCommandHandler(
            ITemporaryUserWriteRepository temporaryUserWriteRepository,
            ITemporaryUserReadRepository temporaryUserReadRepository,
            UserManager<Domain.Entities.Identity.AppUser> userManager,
            IMailService mailService)
        {
            _temporaryUserWriteRepository = temporaryUserWriteRepository;
            _temporaryUserReadRepository = temporaryUserReadRepository;
            _userManager = userManager;
            _mailService = mailService;
        }
        public async Task<RegisterUserCommandResponse> Handle(RegisterUserCommandRequest request, CancellationToken cancellationToken)
        {
            DateTime expiresAt = DateTime.UtcNow.AddMinutes(5);
            // 1. Email artıq real istifadecide varsa qeydiyyat mumkun deyil
            var existingUser = await _userManager.FindByEmailAsync(request.Email);
            if (existingUser != null)
            {
                throw new CustomHttpException(409, new
                {
                    error_code = 1001,
                    message = "UserAlreadyActivated"
                });
            }

            // 2. Temporary user varsa, yoxlayiriq
            var existingTemp = await _temporaryUserReadRepository.GetSingleAsync(x => x.Email == request.Email && !x.IsDeleted);

            if (existingTemp != null)
            {
                // Eger kod hele de etibarlidirsa, yeniden gondermesini onleyirik
                if (existingTemp.ExpiresAt > DateTime.UtcNow)
                {
                    throw new CustomHttpException(409, new
                    {
                        error_code = 1002,
                        message = "VerificationCodeAlreadySent",
                        expires_at = existingTemp.ExpiresAt
                    });
                }

                // Eger kodun vaxti kecibse yenileyirik
                int newCode = GenerateVerificationCode();
                existingTemp.SetVerificationCode(newCode, expiresAt);
                existingTemp.SetLastSentAt(DateTime.UtcNow);
                existingTemp.IncrementResendCount();

                _temporaryUserWriteRepository.Update(existingTemp);
                await _temporaryUserWriteRepository.SaveAsync();

                await SendVerificationMailAsync(request.Email, newCode);

                return new()
                {
                    Message = "VerificationCodeSent",
                    ExpiresAt = expiresAt
                };
            }

            // 3. Temporary user yoxdursa yenisi yaradılır
            TemporaryUser tempUser = new();

            int verificationCode = GenerateVerificationCode();

            var tempAppUser = new Domain.Entities.Identity.AppUser { UserName = request.Email, Email = request.Email };
            var passwordHasher = new PasswordHasher<Domain.Entities.Identity.AppUser>();
            string hashedPassword = passwordHasher.HashPassword(tempAppUser, request.Password);

            tempUser.SetDetails(
                request.Name,
                request.Surname,
                request.Email,
                request.Phone,
                hashedPassword,
                expiresAt,
                verificationCode);

            tempUser.SetLastSentAt(DateTime.UtcNow);
            tempUser.IncrementResendCount();

            await _temporaryUserWriteRepository.AddAsync(tempUser);
            await _temporaryUserWriteRepository.SaveAsync();

            await SendVerificationMailAsync(request.Email, verificationCode);

            return new()
            {
                Message = "RegistrationCompleted",
                ExpiresAt = expiresAt
            };
        }

        private async Task SendVerificationMailAsync(string email, int code)
        {
            string subject = "Email Təsdiqləmə Kodu";
            string body = $"Qeydiyyatı tamamlamaq üçün aşağıdakı 6 rəqəmli kodu istifadə edin: {code}";

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
