using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace StarterKit.Persistence.EntityTypeConfiguration.TemporaryUser
{
    public class TemporaryUserEntityTypeConfiguration : IEntityTypeConfiguration<Domain.Entities.Identity.TemporaryUser>
    {
        public void Configure(EntityTypeBuilder<Domain.Entities.Identity.TemporaryUser> builder)
        {

            builder.HasKey(i => i.Id);
            builder.Property(m => m.Id).ValueGeneratedOnAdd();
            builder.Property(i => i.CreatedDate);
            builder.ToTable("temporary_users");
        }
    }
}
