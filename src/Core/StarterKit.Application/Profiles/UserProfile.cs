using AutoMapper;
using StarterKit.Application.DTOs.User;

namespace StarterKit.Application.Profiles
{
    public class UserProfile : Profile
    {
        public UserProfile()
        {
            CreateMap<Domain.Entities.Identity.AppUser, ListUser>()
                .ForMember(dest => dest.Phone,
                          opt => opt.MapFrom(src => src.PhoneNumber));


            CreateMap<Domain.Entities.Identity.AppUser, UserDto>()
                .ForMember(dest => dest.Phone,
                          opt => opt.MapFrom(src => src.PhoneNumber));
        }
    }
}
