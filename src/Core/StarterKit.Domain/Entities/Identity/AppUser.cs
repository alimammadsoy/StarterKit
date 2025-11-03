using Microsoft.AspNetCore.Identity;

namespace StarterKit.Domain.Entities.Identity;

public class AppUser : IdentityUser<int>
{
    public string NameSurname { get; set; }
    public string? RefreshToken { get; set; }
    public DateTime? RefreshTokenEndDate { get; set; }

    public bool IsDeleted { get; set; }
}