using StarterKit.Domain.Entities.Common;
using StarterKit.Domain.Entities.EndpointAggregate;
using StarterKit.Domain.Entities.Identity;

namespace StarterKit.Domain.Entities.MenuAggregate
{
    public class Menu : Editable<AppUser>
    {
        public string Name { get; set; }

        public ICollection<Endpoint> Endpoints { get; set; }
    }
}
