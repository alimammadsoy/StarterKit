namespace StarterKit.Application.Features.Command.AppUser.DTOs
{
    public class JwtTokenDto
    {
        public string Token { get; set; }
        public string RefreshToken { get; set; }
        public DateTime ExpireAt { get; set; }
    }
}
