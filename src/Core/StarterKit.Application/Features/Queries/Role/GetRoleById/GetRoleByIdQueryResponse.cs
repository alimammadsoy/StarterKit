using StarterKit.Application.DTOs.Endpoint;

namespace StarterKit.Application.Features.Queries.Role.GetRoleById
{
    public class GetRoleByIdQueryResponse
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public List<EndpointDto> Permissions { get; set; }
    }
}