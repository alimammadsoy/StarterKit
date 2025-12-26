using StarterKit.Domain.Entities.Common;

namespace StarterKit.Domain.Entities.Identity
{
    public class UserRefreshToken : Entity
    {
        public string RefreshToken { get; private set; }
        public DateTime ExpiresAt { get; private set; }
        public int UserId { get; private set; }
        public AppUser User { get; set; }

        public void SetDetails(string refreshToken, DateTime expiresAt, int userId)
        {
            RefreshToken = refreshToken;
            ExpiresAt = expiresAt;
            UserId = userId;
        }
    }
}