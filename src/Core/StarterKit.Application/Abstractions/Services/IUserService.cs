using StarterKit.Application.DTOs.User;
using StarterKit.Domain.Entities.Identity;

namespace StarterKit.Application.Abstractions.Services
{
    public interface IUserService
    {
        Task<CreateUserResponse> CreateAsync(CreateUser model);
        Task UpdateRefreshTokenAsync(string refreshToken, AppUser user, int refreshTokenLifetimeSeconds);
        Task UpdatePasswordAsync(int userId, string resetToken, string newPassword);
        IQueryable<AppUser> GetAllUsersAsync();
        int TotalUsersCount { get; }
        Task AssignRoleToUserAsnyc(int userId, string[] roles);
        Task<string[]> GetRolesToUserAsync(string userIdOrName);
        Task<bool> HasRolePermissionToEndpointAsync(string name, string code);
    }
}
