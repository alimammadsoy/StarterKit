using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using StarterKit.Application.Abstractions.Services;
using StarterKit.Application.Abstractions.Services.Authentications;
using StarterKit.Application.Repositories;
using StarterKit.Domain.Entities.Identity;
using StarterKit.Persistence.Contexts;
using StarterKit.Persistence.Repositories;
using StarterKit.Persistence.Services;
namespace StarterKit.Persistence
{
    public static class ServiceRegistration
    {
        public static void AddPersistenceServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<ApplicationDbContext>(opt =>
            {
                opt.UseNpgsql(configuration.GetConnectionString("cString"), options =>
                {
                    options.MigrationsHistoryTable("__efmigrationshistory", "production");
                    options.EnableRetryOnFailure(10, TimeSpan.FromSeconds(3), new List<string>());
                }).UseSnakeCaseNamingConvention();
            }, ServiceLifetime.Scoped);
            services.AddIdentity<AppUser, AppRole>(options =>
            {
                options.Password.RequiredLength = 3;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireDigit = false;
                options.Password.RequireLowercase = false;
                options.Password.RequireUppercase = false;
            }).AddEntityFrameworkStores<ApplicationDbContext>()
            .AddDefaultTokenProviders();

            services.Scan(scan => scan
               .FromAssembliesOf(typeof(IReadRepository<>))
               .AddClasses(classes => classes.AssignableTo(typeof(IReadRepository<>)))
               .AsImplementedInterfaces()
               .WithScopedLifetime());

            services.Scan(scan => scan
                .FromAssembliesOf(typeof(IWriteRepository<>))
                .AddClasses(classes => classes.AssignableTo(typeof(IWriteRepository<>)))
                .AsImplementedInterfaces()
                .WithScopedLifetime());

            services.Scan(scan => scan
                .FromAssembliesOf(typeof(ReadRepository<>))
                .AddClasses(classes => classes.AssignableTo(typeof(ReadRepository<>)))
                .AsImplementedInterfaces()
                .WithScopedLifetime());

            services.Scan(scan => scan
                .FromAssembliesOf(typeof(WriteRepository<>))
                .AddClasses(classes => classes.AssignableTo(typeof(WriteRepository<>)))
                .AsImplementedInterfaces()
                .WithScopedLifetime());


            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<IExternalAuthentication, AuthService>();
            services.AddScoped<IInternalAuthentication, AuthService>();
            services.AddScoped<IRoleService, RoleService>();
            services.AddScoped<IAuthorizationEndpointService, AuthorizationEndpointService>();
        }
    }
}
