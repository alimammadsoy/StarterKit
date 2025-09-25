using MediatR;
using StarterKit.Application.Abstractions.Services;
using StarterKit.Application.Consts;
using System.ComponentModel.DataAnnotations;

namespace StarterKit.Application.Features.Commands.AppUser.UpdatePassword
{
    public class UpdatePasswordCommandHandler : IRequestHandler<UpdatePasswordCommandRequest, ResponseDto>
    {
        readonly IUserService _userService;

        public UpdatePasswordCommandHandler(IUserService userService)
        {
            _userService = userService;
        }

        public async Task<ResponseDto> Handle(UpdatePasswordCommandRequest request, CancellationToken cancellationToken)
        {
            await _userService.UpdatePasswordAsync(request.UserId, request.ResetToken, request.Password);
            return new()
            {
                Message = "Şifrə uğurla dəyişdirildi"
            };
        }
    }
}
