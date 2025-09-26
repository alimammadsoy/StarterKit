namespace StarterKit.Application.Abstractions.Services.Authentications
{
    public interface IInternalAuthentication
    {
        Task<DTOs.Auth.JwtTokenDto> LoginAsync(string usernameOrEmail, string password, int accessTokenLifeTime);
        Task<DTOs.Auth.JwtTokenDto> RefreshTokenLoginAsync(string refreshToken);
    }
}
