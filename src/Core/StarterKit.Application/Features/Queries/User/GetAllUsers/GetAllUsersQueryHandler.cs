using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using StarterKit.Application.Abstractions.Services;
using StarterKit.Application.Consts;
using StarterKit.Application.DTOs.User;

namespace StarterKit.Application.Features.Queries.User.GetAllUsers
{
    public class GetAllUsersQueryHandler : IRequestHandler<GetAllUsersQueryRequest, AllDto<ListUser>>
    {
        readonly IUserService _userService;
        private readonly IMapper _mapper;

        public GetAllUsersQueryHandler(IUserService userService, IMapper mapper)
        {
            _userService = userService;
            _mapper = mapper;
        }

        public async Task<AllDto<ListUser>> Handle(GetAllUsersQueryRequest request, CancellationToken cancellationToken)
        {
            var users = _userService.GetAllUsersAsync();

            users = users.ApplySortingAndFiltering(request.ColumnName, request.OrderBy, request.Search);

            int totalCount = await users.CountAsync(cancellationToken);

            int totalPage = request.PageSize != null
                ? (int)Math.Ceiling(totalCount / (double)request.PageSize)
                : 1;

            if (request.PageNumber != null && request.PageSize != null)
            {
                users = users
                    .Skip((request.PageNumber.Value - 1) * request.PageSize.Value)
                    .Take(request.PageSize.Value);
            }

            List<ListUser> userDtos = _mapper.Map<List<ListUser>>(await users.ToListAsync(cancellationToken));

            return new()
            {
                Data = userDtos,
                TotalCount = totalCount,
                TotalPage = totalPage,
            };
        }
    }
}
