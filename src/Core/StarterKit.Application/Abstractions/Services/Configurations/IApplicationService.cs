using StarterKit.Application.DTOs.Configuration;

namespace StarterKit.Application.Abstractions.Services.Configurations
{
    public interface IApplicationService
    {
        List<Menu> GetAuthorizeDefinitionEndpoints(Type type);
    }
}
