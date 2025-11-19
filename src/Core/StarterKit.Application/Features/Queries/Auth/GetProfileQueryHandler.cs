using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using StarterKit.Application.DTOs.Endpoint;
using StarterKit.Application.Exceptions;
using StarterKit.Application.Repositories.Endpoint;
using StarterKit.Domain.Entities.Identity;
using System.Security.Claims;

namespace StarterKit.Application.Features.Queries.Auth
{
    public class GetProfileQueryHandler : IRequestHandler<GetProfileQueryRequest, GetProfileQueryResponse>
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly UserManager<AppUser> _userManager;
        private readonly IEndpointReadRepository _endpointReadRepository;

        public GetProfileQueryHandler(IHttpContextAccessor httpContextAccessor,
                                      UserManager<AppUser> userManager,
                                      IEndpointReadRepository endpointReadRepository)
        {
            _httpContextAccessor = httpContextAccessor;
            _userManager = userManager;
            _endpointReadRepository = endpointReadRepository;
        }

        public async Task<GetProfileQueryResponse> Handle(GetProfileQueryRequest request, CancellationToken cancellationToken)
        {
            var context = _httpContextAccessor.HttpContext;
            if (context == null)
                throw new InvalidOperationException("HttpContext is not available.");

            var principal = context.User;
            if (principal?.Identity == null || !principal.Identity.IsAuthenticated)
                throw new UnauthorizedAccessException();

            AppUser user = null;

            var nameId = principal.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
            if (!string.IsNullOrEmpty(nameId))
                user = await _userManager.FindByIdAsync(nameId);

            if (user == null)
            {
                var username = principal.Identity?.Name;
                if (!string.IsNullOrEmpty(username))
                    user = await _userManager.FindByNameAsync(username);
            }

            if (user == null)
            {
                var email = principal.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;
                if (!string.IsNullOrEmpty(email))
                    user = await _userManager.FindByEmailAsync(email);
            }

            if (user == null)
                throw new NotFoundException("User not found.");

            var roles = (await _userManager.GetRolesAsync(user)).ToList();

            var permissions = await _endpointReadRepository.Table
                .Include(e => e.Roles)
                .Where(e => !e.IsDeleted && e.Roles.Any(r => roles.Contains(r.Name)))
                .Select(e => new EndpointDto { Id = e.Id, Name = e.Definition })
                .ToListAsync(cancellationToken);

            return new GetProfileQueryResponse
            {
                //Id = user.Id,
                Email = user.Email,
                Name = user.Name,
                Surname = user.Surname,
                Phone = user.PhoneNumber,
                Roles = roles,
                Permissions = permissions
            };
        }
    }
}
