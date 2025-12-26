using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.EntityFrameworkCore;
using StarterKit.Application.Abstractions.Services;
using StarterKit.Application.CustomAttributes;
using StarterKit.Application.Repositories.Endpoint;
using System.Security.Claims;
using System.Text.RegularExpressions;

namespace StarterKit.WebApi.Middlewares
{
    public class RolePermissionMiddleware
    {
        private readonly RequestDelegate _next;

        public RolePermissionMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var endpointReadRepository = context.RequestServices.GetRequiredService<IEndpointReadRepository>();
            var authorizationEndpointService = context.RequestServices.GetRequiredService<IAuthorizationEndpointService>();
            var userService = context.RequestServices.GetRequiredService<IUserService>();

            var endpoint = context.GetEndpoint();
            if (endpoint == null)
            {
                await _next(context);
                return;
            }

            // Try to get controller/action descriptor
            var cad = endpoint.Metadata.GetMetadata<ControllerActionDescriptor>();

            // If controller/action not found or AllowAnonymous present allow through
            if (cad == null ||
                endpoint.Metadata.GetMetadata<Microsoft.AspNetCore.Authorization.IAllowAnonymous>() != null)
            {
                await _next(context);
                return;
            }

            // If not authenticated, let the authentication pipeline handle (401)
            var user = context.User;
            if (user?.Identity == null || !user.Identity.IsAuthenticated)
            {
                await _next(context);
                return;
            }

            // Super-admin bypass
            if (user.IsInRole("SuperAdmin") || string.Equals(user.Identity.Name, "admin", System.StringComparison.OrdinalIgnoreCase))
            {
                await _next(context);
                return;
            }

            // Fallback to DB lookup for SuperAdmin
            try
            {
                var idOrName = user.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value ?? "admin";
                if (!string.IsNullOrEmpty(idOrName))
                {
                    var rolesForUser = await userService.GetRolesToUserAsync(idOrName);
                    if (rolesForUser?.Any(r => string.Equals(r, "SuperAdmin", StringComparison.OrdinalIgnoreCase)) == true)
                    {
                        await _next(context);
                        return;
                    }
                }
            }
            catch
            {
                // ignore and continue to normal checks
            }

            // Try persistent match by ControllerName + ActionName
            var controllerFullName = cad.ControllerTypeInfo.FullName;
            var actionName = cad.ActionName;

            var endpointEntity = await endpointReadRepository.Table
                .Include(e => e.Roles)
                .Include(e => e.Menu)
                .FirstOrDefaultAsync(e => e.ControllerName == controllerFullName && e.ActionName == actionName && !e.IsDeleted);

            string[] allowedRoles = null;

            if (endpointEntity != null)
            {
                allowedRoles = endpointEntity.Roles?.Select(r => r.Name).ToArray();
            }
            else
            {
                // Fallback: use AuthorizeDefinitionAttribute on method (or controller)
                var authDef = cad.MethodInfo.GetCustomAttributes(true).OfType<AuthorizeDefinitionAttribute>().FirstOrDefault()
                              ?? cad.ControllerTypeInfo.GetCustomAttributes(true).OfType<AuthorizeDefinitionAttribute>().FirstOrDefault();
                
                if (authDef == null)
                {
                    await _next(context);
                    return;
                }

                var httpMethod = context.Request.Method.ToUpperInvariant();
                var actionType = authDef.ActionType.ToString();
                var definition = string.IsNullOrWhiteSpace(authDef.Definition) ? cad.ActionName : authDef.Definition;
                var code = $"{httpMethod}.{actionType}.{NormalizeDefinition(definition)}";
                var menu = authDef.Menu;

                var roles = await authorizationEndpointService.GetRolesToEndpointAsync(code, menu);
                if (roles != null)
                    allowedRoles = roles.ToArray();
            }

            // If no roles configured -> deny (403). Change policy if you prefer default allow.
            if (allowedRoles == null || allowedRoles.Length == 0)
            {
                context.Response.StatusCode = StatusCodes.Status403Forbidden;
                return;
            }

            // Fast check with claims
            if (allowedRoles.Any(r => user.IsInRole(r)))
            {
                await _next(context);
                return;
            }

            // Fallback to DB roles for the current user (use NameIdentifier claim first)
            var userIdClaim = user.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
            string[] userRoles = Array.Empty<string>();

            try
            {
                if (!string.IsNullOrEmpty(userIdClaim))
                    userRoles = await userService.   GetRolesToUserAsync(userIdClaim);
                else if (!string.IsNullOrEmpty(user.Identity.Name))
                    userRoles = await userService.GetRolesToUserAsync(user.Identity.Name);
            }
            catch
            {
                // ignore lookup errors, treat as not authorized
            }

            if (userRoles.Intersect(allowedRoles).Any())
            {
                await _next(context);
                return;
            }

            // Not authorized
            context.Response.StatusCode = StatusCodes.Status403Forbidden;
            return;
        }

        private static string NormalizeDefinition(string definition)
        {
            if (string.IsNullOrWhiteSpace(definition))
                return string.Empty;
            var cleaned = Regex.Replace(definition, @"\s+", "");
            cleaned = Regex.Replace(cleaned, @"[^0-9A-Za-z_\.]", "");
            return cleaned;
        }
    }
}
