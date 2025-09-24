using StarterKit.Domain.Entities.Identity;

namespace StarterKit.Domain.Entities.Common;

public class Auditable<TUser> : Entity where TUser : AppUser
{
    public int CreatedById { get; protected set; }
    //  public DateTime RecordDateTime { get; protected set; }

    public void SetAuditDetails(int createdById)
    {
        if (CreatedById != 0 && CreatedById != createdById)
        {
            //   throw new DomainException("CreatedBy already set.");
        }
        CreatedById = createdById;
        //  RecordDateTime = DateTime.UtcNow.AddHours(4);
    }
}