using Microsoft.AspNetCore.Authentication;

namespace StarterKit.WebApi.Middlewares
{
    public static class ConfigureMiddlewares
    {
        public static void UseMiddlewares(this IApplicationBuilder app)
        {
            app.UseMiddleware<AuthenticationMiddleware>();
            app.UseMiddleware<ExceptionHandlingMiddleware>();  
        }
    }
}
