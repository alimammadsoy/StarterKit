using StarterKit.Application.DTOs.Endpoint;

namespace StarterKit.Application.Features.Queries.Auth
{
    public class GetProfileQueryResponse
    {
        //public int Id { get; set; }
        public string Email { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public string Phone { get; set; }
        public List<string> Roles { get; set; }
        public List<EndpointDto> Permissions { get; set; }
    }
}
