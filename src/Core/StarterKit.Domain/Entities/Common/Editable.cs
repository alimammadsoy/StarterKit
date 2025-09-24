using StarterKit.Domain.Entities.Identity;
using System.Security.AccessControl;

namespace StarterKit.Domain.Entities.Common;

public class Editable<TUser> : Auditable<TUser> where TUser : AppUser
{
    public int? UpdateById { get; protected set; }
    public DateTime? LastUpdateDateTime { get; protected set; }

    public void SetEditFields(int? updatedById)
    {

        UpdateById = updatedById;
        LastUpdateDateTime = DateTime.UtcNow;
    }
}