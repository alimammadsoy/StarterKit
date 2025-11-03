using StarterKit.Domain.Entities.Identity;

namespace StarterKit.Application.Abstractions.Services
{
    public interface IRoleService
    {
        IQueryable<AppRole> GetAllRolesAsync();
        Task<AppRole> GetRoleById(int id);
        Task<bool> CreateRole(string name);
        Task<bool> DeleteRole(int id);
        Task<bool> UpdateRole(int id, string name);
    }
}
