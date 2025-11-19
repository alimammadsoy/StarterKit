using StarterKit.Application.DTOs.Role;

namespace StarterKit.Application.DTOs.User
{
    public class ListUser
    {
        public int Id { get; set; }
        public string Email { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        //public string UserName { get; set; }
        public string Phone { get; set; }
        //public bool TwoFactorEnabled { get; set; }

        //public List<RoleDto> Roles { get; set; }
    }
}
