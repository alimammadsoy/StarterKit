using MediatR;
using Microsoft.AspNetCore.Identity;
using StarterKit.Application.Consts;
using StarterKit.Application.Exceptions;
using StarterKit.Application.Repositories.TemporaryUser;

namespace StarterKit.Application.Features.Command.AppUser.VerifyEmail
{
    public class VerifyEmailCommandHandler : IRequestHandler<VerifyEmailCommandRequest, ResponseDto>
    {
        private readonly ITemporaryUserReadRepository _temporaryUserReadRepository;
        private readonly ITemporaryUserWriteRepository _temporaryUserWriteRepository;
        private readonly UserManager<Domain.Entities.Identity.AppUser> _userManager;

        public VerifyEmailCommandHandler(
            ITemporaryUserReadRepository temporaryUserReadRepository,
            ITemporaryUserWriteRepository temporaryUserWriteRepository,
            UserManager<Domain.Entities.Identity.AppUser> userManager)
        {
            _temporaryUserReadRepository = temporaryUserReadRepository;
            _temporaryUserWriteRepository = temporaryUserWriteRepository;
            _userManager = userManager;
        }

        public async Task<ResponseDto> Handle(VerifyEmailCommandRequest request, CancellationToken cancellationToken)
        {
            // 1️. Temporary user tapılır
            var tempUser = await _temporaryUserReadRepository
                .GetSingleAsync(x => x.Email == request.Email && !x.IsDeleted);

            if (tempUser == null)
                throw new BadRequestException("NotRegistered");

            // 2️. Kod düzgünlük yoxlaması
            if (tempUser.VerificationCode != request.VerificationCode)
                throw new BadRequestException("InvalidVerificationCode");

            // 3️. Kodun müddəti bitib-bitmədiyini yoxlayırıq
            if (DateTime.UtcNow > tempUser.ExpiresAt)
                throw new BadRequestException("VerificationCodeExpired");

            // 4️. Yeni AppUser yaradılır
            var newUser = new Domain.Entities.Identity.AppUser
            {
                UserName = tempUser.Email,
                Email = tempUser.Email,
                PhoneNumber = tempUser.Phone,
                Name = tempUser.Name,
                Surname = tempUser.Surname
            };

            var result = await _userManager.CreateAsync(newUser);

            if (!result.Succeeded)
                throw new Exception(string.Join("; ", result.Errors.Select(e => e.Description)));

            newUser.PasswordHash = tempUser.PasswordHash;
            await _userManager.UpdateAsync(newUser);

            // 5️. Temporary record silinir
            await _temporaryUserWriteRepository.RemoveAsync(tempUser.Id);
            await _temporaryUserWriteRepository.SaveAsync();

            // 6️. Mesaj qaytarılır
            return new()
            {
                Message = "EmailVerified"
            };
        }
    }
}
