using StarterKit.Domain.Entities.Common;

namespace StarterKit.Domain.Entities.Identity
{
    public class PasswordReset : Editable<AppUser>
    {
        public int UserId { get; private set; }
        public int Code { get; private set; }
        public DateTime ExpiresAt { get; private set; }
        public AppUser User { get; set; }

        public void SetDetails(int userId, int code, DateTime expiresAt)
        {
            UserId = userId;
            Code = code;
            ExpiresAt = expiresAt;
        }
    }
}