using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace StarterKit.Persistence.EntityTypeConfiguration.PasswordReset
{
    public class PasswordResetEntityTypeConfiguration : IEntityTypeConfiguration<Domain.Entities.Identity.PasswordReset>
    {
        public void Configure(EntityTypeBuilder<Domain.Entities.Identity.PasswordReset> builder)
        {
            builder.HasKey(i => i.Id);
            builder.Property(m => m.Id).ValueGeneratedOnAdd();
            builder.Property(i => i.CreatedDate);
            builder.ToTable("password_resets");
        }
    }
}
