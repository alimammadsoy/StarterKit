namespace StarterKit.Application.Abstractions.Services
{
    public interface IAuthorizationEndpointService
    {
        public Task AssignRoleEndpointAsync(string[] roles, string menu, string code, Type type);
        public Task<List<string>> GetRolesToEndpointAsync(string code, string menu);

        // New: sync discovered endpoints into the DB (upsert)
        public Task SyncEndpointsAsync();

        // New: assign endpoints (by endpointId) to a role (replaces existing endpoint assignments for that role)
        public Task AssignEndpointsToRoleAsync(int roleId, int[] endpointIds);
    }
}
