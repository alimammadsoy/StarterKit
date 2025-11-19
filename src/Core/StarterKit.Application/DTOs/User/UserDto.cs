using StarterKit.Application.DTOs.Role;

namespace StarterKit.Application.DTOs.User
{
    public class UserDto
    {
        public int Id { get; set; }
        public string Email { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public string Phone { get; set; }

        public List<RoleDto> Roles { get; set; }
    }
}
