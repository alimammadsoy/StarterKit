using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using StarterKit.Application.Behaviors;
using StarterKit.Application.Pipelines.Validation;
using System.Reflection;

namespace StarterKit.Application
{
    public static class ServiceRegistration
    {
        public static void AddApplicationServices(this IServiceCollection services)
        {
            services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());
            services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblies(Assembly.GetExecutingAssembly()));
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(RequestValidationBehavior<,>));
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(LoggingBehavior<,>));

            services.AddAutoMapper(Assembly.GetExecutingAssembly());
        }
    }
}
