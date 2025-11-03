using MediatR;
using Microsoft.AspNetCore.Identity;
using StarterKit.Application.Consts;
using StarterKit.Application.Exceptions;

namespace StarterKit.Application.Features.Command.AppUser.DeleteUser
{
    public class DeleteUserCommandHandler : IRequestHandler<DeleteUserCommandRequest, ResponseDto>
    {
        private readonly UserManager<Domain.Entities.Identity.AppUser> _userManager;

        public DeleteUserCommandHandler(UserManager<Domain.Entities.Identity.AppUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task<ResponseDto> Handle(DeleteUserCommandRequest request, CancellationToken cancellationToken)
        {
            var user = await _userManager.FindByIdAsync(request.Id.ToString());
            if (user == null)
                throw new NotFoundException("İstifadəçi tapılmadı");

            user.IsDeleted = true;
            var result = await _userManager.UpdateAsync(user);

            if (!result.Succeeded)
            {
                var errors = string.Join("; ", result.Errors.Select(e => e.Description));
                return new ResponseDto { Message = $"İstifadəçi silinərkən xəta: {errors}" };
            }

            return new ResponseDto { Message = "İstifadəçi uğurla silindi" };
        }
    }
}
