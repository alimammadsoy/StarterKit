using StarterKit.Domain.Entities.Common;
using StarterKit.Domain.Entities.Identity;
using StarterKit.Domain.Entities.MenuAggregate;

namespace StarterKit.Domain.Entities.EndpointAggregate
{
    public class Endpoint : Editable<AppUser>
    {
        public Endpoint()
        {
            Roles = new HashSet<AppRole>();
        }
        public string ActionType { get; set; }
        public string HttpType { get; set; }
        public string Definition { get; set; }
        public string Code { get; set; }

        public Menu Menu { get; set; }
        public ICollection<AppRole> Roles { get; set; }
    }
}

//{ "actionType":"Updating","httpType":"PUT","definition":"Update Role","code":"PUT.Updating.UpdateRole"}