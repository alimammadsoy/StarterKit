using AutoMapper;
using StarterKit.Application.DTOs.Role;
using StarterKit.Domain.Entities.Identity;

namespace StarterKit.Application.Profiles
{
    public class RoleProfile : Profile
    {
        public RoleProfile()
        {
            CreateMap<AppRole, RoleDto>()
                .ForMember(dest => dest.PermissionsCount,
                    opt => opt.MapFrom(src => src.Endpoints.Count));
        }
    }
}
