using StarterKit.Domain.Entities.Common;

namespace StarterKit.Domain.Entities.Identity
{
    public class TemporaryUser : Editable<AppUser>
    {
        public string Name { get; private set; }
        public string Surname { get; private set; }
        public string Email { get; private set; }
        public string Phone { get; private set; }
        public string PasswordHash { get; private set; }
        public int VerificationCode { get; private set; }
        public DateTime ExpiresAt { get; private set; }
        public DateTime? LastSentAt { get; private set; }
        public int ResendCount { get; private set; }

        public void SetDetails(string name, string surname, string email, string phone, string passwordHash, DateTime expiresAt, int code)
        {
            Name = name;
            Surname = surname;
            Email = email;
            Phone = phone;
            PasswordHash = passwordHash;
            ExpiresAt = expiresAt;
            VerificationCode = code;
        }

        public void SetVerificationCode(int code, DateTime expiresAt)
        {
            VerificationCode = code;
            ExpiresAt = expiresAt;
        }
        public void SetLastSentAt(DateTime? sentAt)
        {
            LastSentAt = sentAt;
        }

        public void IncrementResendCount()
        {
            ResendCount++;
        }
    }
}