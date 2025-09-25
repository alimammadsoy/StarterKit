using StarterKit.Application.Repositories.Endpoint;
using StarterKit.Persistence.Contexts;

namespace StarterKit.Persistence.Repositories.Endpoint
{
    public class EndpointReadRepository : ReadRepository<Domain.Entities.EndpointAggregate.Endpoint>, IEndpointReadRepository
    {
        public EndpointReadRepository(ApplicationDbContext context) : base(context)
        {
        }
    }
}
