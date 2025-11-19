using StarterKit.Application.DTOs.User;

namespace StarterKit.Application.Features.Queries.User.GetAllUsers
{
    public class GetAllUsersQueryResponse
    {
        public List<ListUser> Users { get; set; }
        public int TotalCount { get; set; }
    }
}