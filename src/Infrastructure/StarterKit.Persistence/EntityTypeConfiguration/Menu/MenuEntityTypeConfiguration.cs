using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace StarterKit.Persistence.EntityTypeConfiguration.Menu
{
    public class MenuEntityTypeConfiguration : IEntityTypeConfiguration<StarterKit.Domain.Entities.MenuAggregate.Menu>
    {
        public void Configure(EntityTypeBuilder<Domain.Entities.MenuAggregate.Menu> builder)
        {
            throw new NotImplementedException();
        }
    }
}
