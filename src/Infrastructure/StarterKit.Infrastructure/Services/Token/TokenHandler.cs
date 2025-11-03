using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using StarterKit.Application.Abstractions.Token;
using StarterKit.Domain.Entities.Identity;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace StarterKit.Infrastructure.Services.Token
{
    public class TokenHandler : ITokenHandler
    {
        readonly IConfiguration _configuration;
        private readonly UserManager<AppUser> _userManager;

        public TokenHandler(IConfiguration configuration, UserManager<AppUser> userManager)
        {
            _configuration = configuration;
            _userManager = userManager;
        }

        public async Task<Application.DTOs.Auth.JwtTokenDto> CreateAccessToken(int second, AppUser user)
        {

            var jwtSettings = _configuration.GetSection("JWT");
            Application.DTOs.Auth.JwtTokenDto token = new();

            SymmetricSecurityKey securityKey = new(Encoding.UTF8.GetBytes(jwtSettings["SecretKey"]));
            SigningCredentials signingCredentials = new(securityKey, SecurityAlgorithms.HmacSha256);

            token.Expiration = DateTime.UtcNow.AddSeconds(second);

            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString())
            };

            if (!string.IsNullOrEmpty(user.UserName))
                claims.Add(new Claim(ClaimTypes.Name, user.UserName));

            // Synchronous/blocking role lookup (quick fix). Prefer async refactor below.
            var roles = await _userManager.GetRolesAsync(user);
            if (roles != null && roles.Any())
            {
                // Add ClaimTypes.Role so IsInRole works with default RoleClaimType
                claims.AddRange(roles.Select(r => new Claim(ClaimTypes.Role, r)));
                // Optionally also add "role" for clients that expect that claim name
                claims.AddRange(roles.Select(r => new Claim("role", r)));
            }

            JwtSecurityToken securityToken = new(
                audience: jwtSettings["Audience"],
                issuer: jwtSettings["Issuer"],
                expires: token.Expiration,
                notBefore: DateTime.UtcNow,
                signingCredentials: signingCredentials,
                claims: claims
                );

            //Token oluşturucu sınıfından bir örnek alalım.
            JwtSecurityTokenHandler tokenHandler = new();
            token.AccessToken = tokenHandler.WriteToken(securityToken);

            //string refreshToken = CreateRefreshToken();

            token.RefreshToken = CreateRefreshToken();
            return token;
        }

        public string CreateRefreshToken()
        {
            byte[] number = new byte[32];
            using RandomNumberGenerator random = RandomNumberGenerator.Create();
            random.GetBytes(number);
            return Convert.ToBase64String(number);
        }
    }
}
