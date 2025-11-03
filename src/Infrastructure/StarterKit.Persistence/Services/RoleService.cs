using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using StarterKit.Application.Abstractions.Services;
using StarterKit.Application.Exceptions;
using StarterKit.Domain.Entities.Identity;

namespace StarterKit.Persistence.Services
{
    public class RoleService : IRoleService
    {
        readonly RoleManager<AppRole> _roleManager;

        public RoleService(RoleManager<AppRole> roleManager)
        {
            _roleManager = roleManager;
        }

        public async Task<bool> CreateRole(string name)
        {
            IdentityResult result = await _roleManager.CreateAsync(new() { Name = name });

            return result.Succeeded;
        }

        public async Task<bool> DeleteRole(int id)
        {
            AppRole appRole = await _roleManager.FindByIdAsync(id.ToString());
            if (appRole == null)
                throw new NotFoundException("Rol tapılmadı");
            IdentityResult result = await _roleManager.DeleteAsync(appRole);
            return result.Succeeded;
        }

        public IQueryable<AppRole> GetAllRolesAsync()
        {
            return _roleManager.Roles.OrderBy(r => r.Id);
        }

        public async Task<AppRole> GetRoleById(int id)
        {
            var role = await _roleManager.Roles
                .Include(r => r.Endpoints)
                .FirstOrDefaultAsync(r => r.Id == id);

            if (role == null)
                throw new NotFoundException("Rol tapılmadı");

            return role;
        }

        public async Task<bool> UpdateRole(int id, string name)
        {
            AppRole role = await _roleManager.FindByIdAsync(id.ToString());

            if (role == null)
                throw new NotFoundException("Rol tapılmadı");

            role.Name = name;
            IdentityResult result = await _roleManager.UpdateAsync(role);
            return result.Succeeded;
        }
    }
}
