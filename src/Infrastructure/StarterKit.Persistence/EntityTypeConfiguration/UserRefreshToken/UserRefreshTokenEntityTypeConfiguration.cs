using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace StarterKit.Persistence.EntityTypeConfiguration.UserRefreshToken
{
    public class UserRefreshTokenEntityTypeConfiguration : IEntityTypeConfiguration<Domain.Entities.Identity.UserRefreshToken>
    {
        public void Configure(EntityTypeBuilder<Domain.Entities.Identity.UserRefreshToken> builder)
        {

            builder.HasKey(i => i.Id);
            builder.Property(m => m.Id).ValueGeneratedOnAdd();
            builder.HasOne(urt => urt.User)
                .WithMany(u => u.RefreshTokens)
                .HasForeignKey(urt => urt.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.ToTable("user_refresh_tokens");
        }
    }
}
