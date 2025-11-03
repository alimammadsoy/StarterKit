using AutoMapper;
using StarterKit.Application.DTOs.Endpoint;
using StarterKit.Domain.Entities.EndpointAggregate;

namespace StarterKit.Application.Profiles
{
    public class EndpointProfile : Profile
    {
        public EndpointProfile()
        {
            CreateMap<Endpoint, EndpointDto>()
               .ForMember(dest => dest.Name,
                          opt => opt.MapFrom(src => src.Definition));
        }
    }
}
