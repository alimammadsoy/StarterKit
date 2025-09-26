using StarterKit.Domain.Entities.Identity;

namespace StarterKit.Application.Abstractions.Token
{
    public interface ITokenHandler
    {
        DTOs.Auth.JwtTokenDto CreateAccessToken(int second, AppUser appUser);
        string CreateRefreshToken();
    }
}
