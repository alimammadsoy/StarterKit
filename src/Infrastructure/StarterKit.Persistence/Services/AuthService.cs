using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using StarterKit.Application.Abstractions.Services;
using StarterKit.Application.Abstractions.Token;
using StarterKit.Application.DTOs.Auth;
using StarterKit.Application.Exceptions;
using StarterKit.Application.Helpers;
using StarterKit.Application.Repositories.UserRefreshToken;
using StarterKit.Domain.Entities.Identity;
using System.Text;

namespace StarterKit.Persistence.Services
{
    public class AuthService : IAuthService
    {
        readonly HttpClient _httpClient;
        readonly IConfiguration _configuration;
        readonly UserManager<Domain.Entities.Identity.AppUser> _userManager;
        readonly ITokenHandler _tokenHandler;
        readonly SignInManager<Domain.Entities.Identity.AppUser> _signInManager;
        readonly IUserService _userService;
        readonly IMailService _mailService;
        readonly IUserRefreshTokenReadRepository _userRefreshTokenRead;
        readonly IUserRefreshTokenWriteRepository _userRefreshTokenWrite;

        public AuthService(IHttpClientFactory httpClientFactory,
            IConfiguration configuration,
            UserManager<Domain.Entities.Identity.AppUser> userManager,
            ITokenHandler tokenHandler,
            SignInManager<AppUser> signInManager,
            IUserService userService,
            IMailService mailService,
            IUserRefreshTokenReadRepository userRefreshTokenRead,
            IUserRefreshTokenWriteRepository userRefreshTokenWrite)
        {
            _httpClient = httpClientFactory.CreateClient();
            _configuration = configuration;
            _userManager = userManager;
            _tokenHandler = tokenHandler;
            _signInManager = signInManager;
            _userService = userService;
            _mailService = mailService;
            _userRefreshTokenRead = userRefreshTokenRead;
            _userRefreshTokenWrite = userRefreshTokenWrite;
        }

        async Task<JwtTokenDto> CreateUserExternalAsync(AppUser user, string email, string name, string surname, UserLoginInfo info, int accessTokenLifeTime)
        {
            bool result = user != null;
            if (user == null)
            {
                user = await _userManager.FindByEmailAsync(email);
                if (user == null)
                {
                    user = new()
                    {
                        Email = email,
                        UserName = email,
                        Name = name,
                        Surname = surname,
                    };
                    var identityResult = await _userManager.CreateAsync(user);
                    result = identityResult.Succeeded;
                }
            }

            if (result)
            {
                await _userManager.AddLoginAsync(user, info); //AspNetUserLogins

                JwtTokenDto token = await _tokenHandler.CreateAccessToken(accessTokenLifeTime, user);
                await _userService.UpdateRefreshTokenAsync(token.RefreshToken, user, Convert.ToInt32(_configuration["JWT:RefreshExpireAt"]));
                return token;
            }
            throw new Exception("Invalid external authentication.");
        }
        /*public async Task<Token> FacebookLoginAsync(string authToken, int accessTokenLifeTime)
        {
            string accessTokenResponse = await _httpClient.GetStringAsync($"https://graph.facebook.com/oauth/access_token?client_id={_configuration["ExternalLoginSettings:Facebook:Client_ID"]}&client_secret={_configuration["ExternalLoginSettings:Facebook:Client_Secret"]}&grant_type=client_credentials");

            FacebookAccessTokenResponse? facebookAccessTokenResponse = JsonSerializer.Deserialize<FacebookAccessTokenResponse>(accessTokenResponse);

            string userAccessTokenValidation = await _httpClient.GetStringAsync($"https://graph.facebook.com/debug_token?input_token={authToken}&access_token={facebookAccessTokenResponse?.AccessToken}");

            FacebookUserAccessTokenValidation? validation = JsonSerializer.Deserialize<FacebookUserAccessTokenValidation>(userAccessTokenValidation);

            if (validation?.Data.IsValid != null)
            {
                string userInfoResponse = await _httpClient.GetStringAsync($"https://graph.facebook.com/me?fields=email,name&access_token={authToken}");

                FacebookUserInfoResponse? userInfo = JsonSerializer.Deserialize<FacebookUserInfoResponse>(userInfoResponse);

                var info = new UserLoginInfo("FACEBOOK", validation.Data.UserId, "FACEBOOK");
                Domain.Entities.Identity.AppUser user = await _userManager.FindByLoginAsync(info.LoginProvider, info.ProviderKey);

                return await CreateUserExternalAsync(user, userInfo.Email, userInfo.Name, info, accessTokenLifeTime);
            }
            throw new Exception("Invalid external authentication.");
        }*/
        /*public async Task<Token> GoogleLoginAsync(string idToken, int accessTokenLifeTime)
        {
            var settings = new GoogleJsonWebSignature.ValidationSettings()
            {
                Audience = new List<string> { _configuration["ExternalLoginSettings:Google:Client_ID"] }
            };

            var payload = await GoogleJsonWebSignature.ValidateAsync(idToken, settings);

            var info = new UserLoginInfo("GOOGLE", payload.Subject, "GOOGLE");
            Domain.Entities.Identity.AppUser user = await _userManager.FindByLoginAsync(info.LoginProvider, info.ProviderKey);

            return await CreateUserExternalAsync(user, payload.Email, payload.Name, info, accessTokenLifeTime);
        }*/

        public async Task<JwtTokenDto> LoginAsync(string usernameOrEmail, string password, int accessTokenLifeTime)
        {
            Domain.Entities.Identity.AppUser user = await _userManager.FindByNameAsync(usernameOrEmail);
            if (user == null)
                user = await _userManager.FindByEmailAsync(usernameOrEmail);

            if (user == null)
                throw new NotFoundException("İstifadəçi tapılmadı");

            if (user.IsDeleted)
                throw new NotFoundException("İstifadəçi tapılmadı");

            SignInResult result = await _signInManager.CheckPasswordSignInAsync(user, password, false);
            if (result.Succeeded) //Authentication başarılı!
            {
                JwtTokenDto token = await _tokenHandler.CreateAccessToken(accessTokenLifeTime, user);
                await _userService.UpdateRefreshTokenAsync(token.RefreshToken, user, Convert.ToInt32(_configuration["JWT:RefreshExpireAt"]));
                return token;
            }
            throw new UnAuthorizedException("Email və ya şifrə yanlışdır");
        }

        public async Task<JwtTokenDto> RefreshTokenLoginAsync(string refreshToken)
        {
            var userRefreshToken = await _userRefreshTokenRead.GetSingleAsync(rt => rt.RefreshToken == refreshToken && !rt.IsDeleted,
                include => include.Include(rt => rt.User));

            if (userRefreshToken == null)
                throw new UnAuthorizedException("InvalidRefreshToken");

            var user = userRefreshToken.User;

            if (userRefreshToken.ExpiresAt <= DateTime.UtcNow || user.IsDeleted)
            {
                await _userRefreshTokenWrite.RemoveAsync(userRefreshToken.Id);
                await _userRefreshTokenWrite.SaveAsync();
                throw new UnAuthorizedException("InvalidRefreshToken");
            }

            JwtTokenDto token = await _tokenHandler.CreateAccessToken(Convert.ToInt32(_configuration["JWT:ExpireAt"]), user);

            await _userService.UpdateRefreshTokenAsync(token.RefreshToken, user, Convert.ToInt32(_configuration["JWT:RefreshExpireAt"]));

            return token;
        }


        public async Task PasswordResetAsnyc(string email)
        {
            AppUser user = await _userManager.FindByEmailAsync(email);
            if (user != null)
            {
                string resetToken = await _userManager.GeneratePasswordResetTokenAsync(user);

                byte[] tokenBytes = Encoding.UTF8.GetBytes(resetToken);
                resetToken = WebEncoders.Base64UrlEncode(tokenBytes);
                resetToken = resetToken.UrlEncode();

                await _mailService.SendPasswordResetMailAsync(email, user.Id, resetToken);
            }
        }

        public async Task<bool> VerifyResetTokenAsync(string resetToken, string userId)
        {
            AppUser user = await _userManager.FindByIdAsync(userId);
            if (user != null)
            {
                //byte[] tokenBytes = WebEncoders.Base64UrlDecode(resetToken);
                //resetToken = Encoding.UTF8.GetString(tokenBytes);
                resetToken = resetToken.UrlDecode();

                return await _userManager.VerifyUserTokenAsync(user, _userManager.Options.Tokens.PasswordResetTokenProvider, "ResetPassword", resetToken);
            }
            return false;
        }
    }
}
