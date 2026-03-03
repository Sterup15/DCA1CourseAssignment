namespace VEA.Core.Domain.Common.Bases;

using System;

public abstract class Entity<TId>
{
    public virtual TId Id { get; protected set; }

    protected Entity() { }

    protected Entity(TId id)
    {
        Id = id;
    }

    public override bool Equals(object obj)
    {
        if (obj is not Entity<TId> other)
            return false;

        if (ReferenceEquals(this, other))
            return true;

        if (GetType() != other.GetType())
            return false;

        if (IsTransient() || other.IsTransient())
            return false;

        return Id.Equals(other.Id);
    }

    public override int GetHashCode()
    {
        if (IsTransient())
            return base.GetHashCode();

        return HashCode.Combine(GetType(), Id);
    }

    public static bool operator ==(Entity<TId> a, Entity<TId> b)
    {
        if (ReferenceEquals(a, b))
            return true;

        if (a is null || b is null)
            return false;

        return a.Equals(b);
    }

    public static bool operator !=(Entity<TId> a, Entity<TId> b)
        => !(a == b);

    private bool IsTransient()
        => Id == null || Id.Equals(default(TId));
}