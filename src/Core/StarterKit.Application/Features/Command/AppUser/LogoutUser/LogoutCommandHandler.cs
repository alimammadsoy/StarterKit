using MediatR;
using StarterKit.Application.Consts;
using StarterKit.Application.Exceptions;
using StarterKit.Application.Repositories.UserRefreshToken;

namespace StarterKit.Application.Features.Command.AppUser.LogoutUser
{
    public class LogoutCommandHandler : IRequestHandler<LogoutCommandRequest, ResponseDto>
    {
        private readonly IUserRefreshTokenWriteRepository _refreshTokenWriteRepository;
        private readonly IUserRefreshTokenReadRepository _refreshTokenReadRepository;

        public LogoutCommandHandler(
            IUserRefreshTokenWriteRepository refreshTokenWriteRepository,
            IUserRefreshTokenReadRepository refreshTokenReadRepository)
        {
            _refreshTokenWriteRepository = refreshTokenWriteRepository;
            _refreshTokenReadRepository = refreshTokenReadRepository;
        }

        public async Task<ResponseDto> Handle(LogoutCommandRequest request, CancellationToken cancellationToken)
        {
            var token = await _refreshTokenReadRepository
                .GetSingleAsync(x => x.RefreshToken == request.RefreshToken && !x.IsDeleted);

            if(token == null)
                throw new UnAuthorizedException("InvalidRefreshToken");

            await _refreshTokenWriteRepository.RemoveAsync(token.Id);
            await _refreshTokenWriteRepository.SaveAsync();

            return new ResponseDto
            {
                Message = "SuccessfullyLoggedOut"
            };
        }
    }
}
