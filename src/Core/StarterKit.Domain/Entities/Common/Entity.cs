namespace StarterKit.Domain.Entities.Common;

public abstract class Entity
{
    public Guid Id { get; set; }
    public bool IsDeleted { get; set; }
    public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
    public bool IsTransient()
    {
        return Id == default;
    }

    public override bool Equals(object obj)
    {
        if (!(obj is Entity))
            return false;

        if (ReferenceEquals(this, obj))
            return true;

        if (GetType() != obj.GetType())
            return false;

        var item = (Entity)obj;

        if (item.IsTransient() || IsTransient())
            return false;
        return item.Id == Id;
    }

    private int? _requestedHashCode;

    public override int GetHashCode()
    {
        if (IsTransient()) return base.GetHashCode();

        if (!_requestedHashCode.HasValue)
            _requestedHashCode =
                Id.GetHashCode() ^
                31; // XOR for random distribution (http://blogs.msdn.com/b/ericlippert/archive/2011/02/28/guidelines-and-rules-for-gethashcode.aspx)

        return _requestedHashCode.Value;

    }

    public static bool operator ==(Entity left, Entity right)
    {
        return left?.Equals(right) ?? Equals(right, null);
    }

    public static bool operator !=(Entity left, Entity right)
    {
        return !(left == right);
    }
}