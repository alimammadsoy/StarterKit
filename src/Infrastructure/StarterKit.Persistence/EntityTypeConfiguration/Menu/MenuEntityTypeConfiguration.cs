using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace StarterKit.Persistence.EntityTypeConfiguration.Menu
{
    public class MenuEntityTypeConfiguration : IEntityTypeConfiguration<Domain.Entities.MenuAggregate.Menu>
    {
        public void Configure(EntityTypeBuilder<Domain.Entities.MenuAggregate.Menu> builder)
        {
            builder.HasKey(e => e.Id);

            builder.Property(e => e.Id).ValueGeneratedOnAdd();
            builder.Property(i => i.CreatedDate);
            builder.ToTable("menus");
        }
    }
}
