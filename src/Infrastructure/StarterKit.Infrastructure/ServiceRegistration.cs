using Microsoft.Extensions.DependencyInjection;
using StarterKit.Application.Abstractions.Services;
using StarterKit.Application.Abstractions.Services.Configurations;
using StarterKit.Application.Abstractions.Services.Storage;
using StarterKit.Application.Abstractions.Token;
using StarterKit.Infrastructure.Services;
using StarterKit.Infrastructure.Services.Configurations;
using StarterKit.Infrastructure.Services.Storage;
using StarterKit.Infrastructure.Services.Token;

namespace StarterKit.Infrastructure
{
    public static class ServiceRegistration
    {
        public static void AddInfrastructureServices(this IServiceCollection serviceCollection)
        {
            serviceCollection.AddScoped<ILocalStorage, LocalStorage>();
            serviceCollection.AddScoped<ITokenHandler, TokenHandler>();
            serviceCollection.AddScoped<IMailService, MailService>();
            serviceCollection.AddScoped<IApplicationService, ApplicationService>();
        }
    }
}
