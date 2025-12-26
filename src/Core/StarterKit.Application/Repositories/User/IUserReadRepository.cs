namespace StarterKit.Application.Repositories.User
{
    public interface IUserReadRepository
    {
        Task<bool> ExistsActiveUserByEmailAsync(string email);
        Task<bool> ExistsActiveUserByUsernameAsync(string username);
    }
}
