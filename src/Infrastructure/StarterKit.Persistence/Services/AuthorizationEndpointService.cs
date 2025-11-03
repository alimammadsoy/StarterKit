using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using StarterKit.Application.Abstractions.Services;
using StarterKit.Application.Abstractions.Services.Configurations;
using StarterKit.Application.Repositories.Endpoint;
using StarterKit.Application.Repositories.Menu;
using StarterKit.Domain.Entities.EndpointAggregate;
using StarterKit.Domain.Entities.Identity;
using StarterKit.Domain.Entities.MenuAggregate;
using System.Reflection;
using StarterKit.Application.CustomAttributes;
using Microsoft.AspNetCore.Mvc;
using System.Text.RegularExpressions;

namespace StarterKit.Persistence.Services
{
    public class AuthorizationEndpointService : IAuthorizationEndpointService
    {
        readonly IApplicationService _applicationService;
        readonly IEndpointReadRepository _endpointReadRepository;
        readonly IEndpointWriteRepository _endpointWriteRepository;
        readonly IMenuReadRepository _menuReadRepository;
        readonly IMenuWriteRepository _menuWriteRepository;
        readonly RoleManager<AppRole> _roleManager;
        public AuthorizationEndpointService(IApplicationService applicationService,
            IEndpointReadRepository endpointReadRepository,
            IEndpointWriteRepository endpointWriteRepository,
            IMenuReadRepository menuReadRepository,
            IMenuWriteRepository menuWriteRepository,
            RoleManager<AppRole> roleManager)
        {
            _applicationService = applicationService;
            _endpointReadRepository = endpointReadRepository;
            _endpointWriteRepository = endpointWriteRepository;
            _menuReadRepository = menuReadRepository;
            _menuWriteRepository = menuWriteRepository;
            _roleManager = roleManager;
        }

        public async Task AssignRoleEndpointAsync(string[] roles, string menu, string code, Type type)
        {
            Menu _menu = await _menuReadRepository.GetSingleAsync(m => m.Name == menu);
            if (_menu == null)
            {
                _menu = new()
                {
                    Name = menu
                };
                await _menuWriteRepository.AddAsync(_menu);

                await _menuWriteRepository.SaveAsync();
            }

            Endpoint? endpoint = await _endpointReadRepository.Table.Include(e => e.Menu).Include(e => e.Roles).FirstOrDefaultAsync(e => e.Code == code && e.Menu.Name == menu);

            if (endpoint == null)
            {
                var action = _applicationService.GetAuthorizeDefinitionEndpoints(type)
                        .FirstOrDefault(m => m.Name == menu)
                        ?.Actions.FirstOrDefault(e => e.Code == code);

                endpoint = new()
                {
                    Code = action.Code,
                    ActionType = action.ActionType,
                    HttpType = action.HttpType,
                    Definition = action.Definition,
                    Menu = _menu
                };

                await _endpointWriteRepository.AddAsync(endpoint);
                await _endpointWriteRepository.SaveAsync();
            }

            foreach (var role in endpoint.Roles)
                endpoint.Roles.Remove(role);

            var appRoles = await _roleManager.Roles.Where(r => roles.Contains(r.Name)).ToListAsync();

            foreach (var role in appRoles)
                endpoint.Roles.Add(role);

            await _endpointWriteRepository.SaveAsync();
        }

        public async Task<List<string>> GetRolesToEndpointAsync(string code, string menu)
        {
            Endpoint? endpoint = await _endpointReadRepository.Table
                .Include(e => e.Roles)
                .Include(e => e.Menu)
                .FirstOrDefaultAsync(e => e.Code == code && e.Menu.Name == menu && !e.IsDeleted);
            if (endpoint != null)
                return endpoint.Roles.Select(r => r.Name).ToList();
            return null;
        }

        public async Task AssignRoleEndpointByIdsAsync(int[] roleIds, int endpointId)
        {
            // Load endpoint and its roles
            Endpoint? endpoint = await _endpointReadRepository.Table
                .Include(e => e.Roles)
                .FirstOrDefaultAsync(e => e.Id == endpointId);

            if (endpoint == null)
                throw new ArgumentException($"Endpoint with id {endpointId} not found.", nameof(endpointId));

            // Remove all existing role relationships
            // If tracked, Clear works; otherwise remove explicitly.
            endpoint.Roles.Clear();

            // Get roles by Id
            var appRoles = await _roleManager.Roles.Where(r => roleIds.Contains(r.Id)).ToListAsync();

            foreach (var role in appRoles)
                endpoint.Roles.Add(role);

            await _endpointWriteRepository.SaveAsync();
        }

        // Helper to create a stable code when needed (same convention you used previously)
        private static string NormalizeDefinition(string definition)
        {
            if (string.IsNullOrWhiteSpace(definition))
                return string.Empty;   

            // remove invalid chars, spaces -> remove or camel-case as you prefer
            var cleaned = Regex.Replace(definition, @"\s+", "");
            cleaned = Regex.Replace(cleaned, @"[^0-9A-Za-z_\.]", "");
            return cleaned;
        }

        // New: scan controllers via reflection + attributes and upsert menus & endpoints
        public async Task SyncEndpointsAsync()
        {
            var controllerTypes = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(a =>
                {
                    try { return a.GetTypes(); }
                    catch { return Array.Empty<Type>(); }
                })
                .Where(t => t.IsClass && t.IsPublic && t.Name.EndsWith("Controller"))
                .ToList();

            foreach (var type in controllerTypes)
            {
                // Iterate public instance methods declared on controller
                var methods = type.GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly);

                foreach (var method in methods)
                {
                    // Look for your AuthorizeDefinition attribute on method
                    var authDef = method.GetCustomAttribute<AuthorizeDefinitionAttribute>();
                    if (authDef == null) continue;

                    var menuName = authDef.Menu;
                    if (string.IsNullOrWhiteSpace(menuName)) continue;

                    // Determine HTTP verb string
                    string httpType;
                    if (method.GetCustomAttribute<HttpPostAttribute>() != null) httpType = "POST";
                    else if (method.GetCustomAttribute<HttpPutAttribute>() != null) httpType = "PUT";
                    else if (method.GetCustomAttribute<HttpDeleteAttribute>() != null) httpType = "DELETE";
                    else if (method.GetCustomAttribute<HttpPatchAttribute>() != null) httpType = "PATCH";
                    else httpType = "GET"; // default if no attribute

                    var actionType = authDef.ActionType.ToString();
                    var definition = string.IsNullOrWhiteSpace(authDef.Definition) ? method.Name : authDef.Definition;
                    var code = $"{httpType}.{actionType}.{NormalizeDefinition(definition)}";

                    // Upsert Menu
                    var menu = await _menuReadRepository.GetSingleAsync(m => m.Name == menuName);
                    if (menu == null)
                    {
                        menu = new Menu { Name = menuName };
                        await _menuWriteRepository.AddAsync(menu);
                        await _menuWriteRepository.SaveAsync();
                    }

                    // Try to find existing endpoint by ControllerName + ActionName + Menu (stable match)
                    var existing = await _endpointReadRepository.Table
                        .Include(e => e.Menu)
                        .Include(e => e.Roles)
                        .FirstOrDefaultAsync(e =>
                            e.ControllerName == type.FullName &&
                            e.ActionName == method.Name &&
                            e.Menu.Name == menuName);

                    // Back-compat: if no stable match, try Code + Menu
                    if (existing == null)
                    {
                        existing = await _endpointReadRepository.Table
                            .Include(e => e.Menu)
                            .Include(e => e.Roles)
                            .FirstOrDefaultAsync(e => e.Code == code && e.Menu.Name == menuName);
                    }

                    if (existing == null)
                    {
                        // create new endpoint row and populate controller/action to avoid duplicates later
                        var newEndpoint = new Endpoint
                        {
                            Code = code,
                            ActionType = actionType,
                            HttpType = httpType,
                            Definition = definition,
                            Menu = menu,
                            ControllerName = type.FullName,
                            ActionName = method.Name
                        };
                        await _endpointWriteRepository.AddAsync(newEndpoint);
                    }
                    else
                    {
                        // Update existing row in-place (keeps Role relationships)
                        existing.Code = code;
                        existing.ActionType = actionType;
                        existing.HttpType = httpType;
                        existing.Definition = definition;
                        existing.Menu = menu;

                        // Set stable keys if missing (this migrates older rows into stable matching)
                        if (string.IsNullOrWhiteSpace(existing.ControllerName))
                            existing.ControllerName = type.FullName;
                        if (string.IsNullOrWhiteSpace(existing.ActionName))
                            existing.ActionName = method.Name;

                        // If the write repo exposes Update, call it (best-effort) otherwise EF change tracking will pick up changes
                        try
                        {
                            _endpointWriteRepository.GetType().GetMethod("Update")?.Invoke(_endpointWriteRepository, new object[] { existing });
                        }
                        catch
                        {
                            // ignore: EF will track changes
                        }
                    }
                }
            }

            await _endpointWriteRepository.SaveAsync();
        }

        public async Task AssignEndpointsToRoleAsync(int roleId, int[] endpointIds)
        {
            // Load the role
            var role = await _roleManager.Roles.FirstOrDefaultAsync(r => r.Id == roleId);
            if (role == null)
                throw new ArgumentException($"Role with id {roleId} not found.", nameof(roleId));

            // Load all endpoints with roles
            var endpoints = await _endpointReadRepository.GetWhere(e => !e.IsDeleted)
                .Include(e => e.Roles)
                .ToListAsync();

            // Ensure endpointIds list is not null
            endpointIds = endpointIds ?? Array.Empty<int>();

            foreach (var endpoint in endpoints)
            {
                var hasRole = endpoint.Roles.Any(r => r.Id == roleId);
                var shouldHave = endpointIds.Contains(endpoint.Id);

                if (shouldHave && !hasRole)
                {
                    // add role
                    endpoint.Roles.Add(role);
                }
                else if (!shouldHave && hasRole)
                {
                    // remove role
                    var existingRole = endpoint.Roles.FirstOrDefault(r => r.Id == roleId);
                    if (existingRole != null)
                        endpoint.Roles.Remove(existingRole);
                }
            }

            await _endpointWriteRepository.SaveAsync();
        }
    }
}
