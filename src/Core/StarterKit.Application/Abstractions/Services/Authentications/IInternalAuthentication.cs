namespace StarterKit.Application.Abstractions.Services.Authentications
{
    public interface IInternalAuthentication
    {
        Task<DTOs.Auth.Token> LoginAsync(string usernameOrEmail, string password, int accessTokenLifeTime);
        Task<DTOs.Auth.Token> RefreshTokenLoginAsync(string refreshToken);
    }
}
