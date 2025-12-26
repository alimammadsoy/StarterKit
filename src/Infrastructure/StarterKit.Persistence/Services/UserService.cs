using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using StarterKit.Application.Abstractions.Services;
using StarterKit.Application.DTOs.User;
using StarterKit.Application.Exceptions;
using StarterKit.Application.Helpers;
using StarterKit.Application.Repositories.Endpoint;
using StarterKit.Application.Repositories.UserRefreshToken;
using StarterKit.Domain.Entities.EndpointAggregate;
using StarterKit.Domain.Entities.Identity;

namespace StarterKit.Persistence.Services
{
    public class UserService : IUserService
    {
        private readonly UserManager<Domain.Entities.Identity.AppUser> _userManager;
        private readonly IEndpointReadRepository _endpointReadRepository;
        private readonly RoleManager<AppRole> _roleManager;
        private readonly IUserRefreshTokenReadRepository _userRefreshTokenRead;
        private readonly IUserRefreshTokenWriteRepository _userRefreshTokenWrite;

        public UserService(UserManager<AppUser> userManager,
            IEndpointReadRepository endpointReadRepository, RoleManager<AppRole> roleManager,
            IUserRefreshTokenReadRepository userRefreshTokenRead, IUserRefreshTokenWriteRepository userRefreshTokenWrite)
        {
            _userManager = userManager;
            _endpointReadRepository = endpointReadRepository;
            _roleManager = roleManager;
            _userRefreshTokenRead = userRefreshTokenRead;
            _userRefreshTokenWrite = userRefreshTokenWrite;
        }

        public async Task<CreateUserResponse> CreateAsync(CreateUser model)
        {
            if (model.Password != model.PasswordConfirm)
            {
                return new CreateUserResponse
                {
                    Succeeded = false,
                    Message = "Şifrə ilə təsdiq şifrəsi eyni deyil"
                };
            }
            IdentityResult result = await _userManager.CreateAsync(new()
            {
                UserName = model.Username,
                Email = model.Email,
                Name = model.Name,
                Surname = model.Surname
            }, model.Password);

            CreateUserResponse response = new() { Succeeded = result.Succeeded };

            if (result.Succeeded)
                response.Message = "İstifadəçi uğurla yaradıldı";
            else
                foreach (var error in result.Errors)
                    response.Message += $"{error.Code} - {error.Description}\n";

            return response;
        }
        /*public async Task UpdateRefreshTokenAsync(string refreshToken, AppUser user, DateTime accessTokenDate, int addOnAccessTokenDate)
        {
            if (user != null)
            {
                user.RefreshToken = refreshToken;
                user.RefreshTokenEndDate = accessTokenDate.AddSeconds(addOnAccessTokenDate);
                await _userManager.UpdateAsync(user);
            }
            else
                throw new NotFoundException();
        }*/

        public async Task UpdateRefreshTokenAsync(string refreshToken, AppUser user, int refreshTokenLifetimeSeconds)
        {
            if (user == null)
                throw new NotFoundException("User not found");

            var oldToken = await _userRefreshTokenRead.GetSingleAsync(x => x.UserId == user.Id && !x.IsDeleted);

            if (oldToken != null)
            {
                await _userRefreshTokenWrite.RemoveAsync(oldToken.Id);
                await _userRefreshTokenWrite.SaveAsync();
            }

            var newRefreshToken = new UserRefreshToken();
            newRefreshToken.SetDetails(refreshToken, DateTime.UtcNow.AddSeconds(refreshTokenLifetimeSeconds), user.Id);

            // 3) Yeni token DB-ə əlavə et
            await _userRefreshTokenWrite.AddAsync(newRefreshToken);
            await _userRefreshTokenWrite.SaveAsync();
        }


        public async Task UpdatePasswordAsync(int userId, string resetToken, string newPassword)

        {
            AppUser user = await _userManager.FindByIdAsync(userId.ToString());
            if (user != null)
            {
                resetToken = resetToken.UrlDecode();
                IdentityResult result = await _userManager.ResetPasswordAsync(user, resetToken, newPassword);
                if (result.Succeeded)
                    await _userManager.UpdateSecurityStampAsync(user);
                else
                    throw new BadRequestException("Şifrələr eyni deyil");
            }
        }

        public IQueryable<AppUser> GetAllUsersAsync()
        {
            return _userManager.Users.Where(u => !u.IsDeleted);
            /*var users = _userManager.Users.Where(u => !u.IsDeleted);
            if (page != null && size != null)
            {
                users = users.Skip((page.Value - 1) * size.Value).Take(size.Value);
            }

            var rolesList = await _roleManager.Roles.ToListAsync();
            var roleByName = rolesList.ToDictionary(r => r.Name, r => r.Id);

            var result = new List<ListUser>();

            foreach (var user in users)
            {
                var roleNames = await _userManager.GetRolesAsync(user);
                var roleDtos = roleNames.Select(rn => new RoleDto
                {
                    Id = roleByName.TryGetValue(rn, out var rid) ? rid : 0,
                    Name = rn
                }).ToList();

                result.Add(new ListUser
                {
                    Id = user.Id,
                    Email = user.Email,
                    Name = user.Name,
                    Surname = user.Surname,
                    Phone = user.PhoneNumber,
                    //TwoFactorEnabled = user.TwoFactorEnabled,
                    //UserName = user.UserName,
                    Roles = roleDtos
                });
            }

            return result;*/
        }

        public int TotalUsersCount => _userManager.Users.Count();

        public async Task AssignRoleToUserAsnyc(int userId, string[] roles)
        {
            AppUser user = await _userManager.Users.FirstOrDefaultAsync(u => u.Id == userId && !u.IsDeleted);

            if (user is null)
                throw new NotFoundException("İstifadəçi tapılmadı");

            var userRoles = await _userManager.GetRolesAsync(user);
            await _userManager.RemoveFromRolesAsync(user, userRoles);

            await _userManager.AddToRolesAsync(user, roles);
        }

        public async Task<string[]> GetRolesToUserAsync(string userIdOrName)
        {
            if (string.IsNullOrWhiteSpace(userIdOrName))
                throw new ArgumentNullException(nameof(userIdOrName));

            AppUser? user = null;

            // Try numeric id first
            if (int.TryParse(userIdOrName, out var id))
            {
                user = await _userManager.Users.FirstOrDefaultAsync(u => u.Id == id);
            }

            // Fallback to username lookup
            if (user == null)
            {
                user = await _userManager.FindByNameAsync(userIdOrName);
            }

            if (user != null)
            {
                var userRoles = await _userManager.GetRolesAsync(user);
                return userRoles.ToArray();
            }

            throw new NotFoundException("İstifadəçi tapılmadı");
        }

        public async Task<bool> HasRolePermissionToEndpointAsync(string name, string code)
        {
            var userRoles = await GetRolesToUserAsync(name);

            if (!userRoles.Any())
                return false;

            Endpoint? endpoint = await _endpointReadRepository.Table
                     .Include(e => e.Roles)
                     .FirstOrDefaultAsync(e => e.Code == code);

            if (endpoint == null)
                return false;
            var endpointRoles = endpoint.Roles.Select(r => r.Name);

            foreach (var userRole in userRoles)
            {
                foreach (var endpointRole in endpointRoles)
                    if (userRole == endpointRole)
                        return true;
            }

            return false;
        }
    }
}
