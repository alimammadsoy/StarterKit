namespace StarterKit.Application.DTOs.Auth
{
    public class JwtTokenDto
    {
        public string AccessToken { get; set; }
        public DateTime Expiration { get; set; }
        public string RefreshToken { get; set; }
    }
}
