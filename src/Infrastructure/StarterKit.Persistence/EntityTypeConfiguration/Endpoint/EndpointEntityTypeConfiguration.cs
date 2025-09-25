using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace StarterKit.Persistence.EntityTypeConfiguration.Endpoint
{
    public class EndpointEntityTypeConfiguration : IEntityTypeConfiguration<Domain.Entities.EndpointAggregate.Endpoint>
    {
        public void Configure(EntityTypeBuilder<Domain.Entities.EndpointAggregate.Endpoint> builder)
        {
            builder.HasKey(e => e.Id);

            builder.Property(e => e.Id).ValueGeneratedOnAdd();
            builder.Property(i => i.CreatedDate);
            builder.ToTable("endpoints");
        }
    }
}
