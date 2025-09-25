using StarterKit.Application.Repositories.Endpoint;
using StarterKit.Persistence.Contexts;

namespace StarterKit.Persistence.Repositories.Endpoint
{
    public class EndpointWriteRepository : WriteRepository<Domain.Entities.EndpointAggregate.Endpoint>, IEndpointWriteRepository
    {
        public EndpointWriteRepository(ApplicationDbContext context) : base(context)
        {
        }
    }
}
