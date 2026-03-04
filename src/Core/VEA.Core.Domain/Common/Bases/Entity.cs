namespace VEA.Core.Domain.Common.Bases;

public abstract class Entity<TId>
{
    public TId Id { get; protected init; } = default!;

    protected Entity() { }
    protected Entity(TId id) => Id = id;

    public override bool Equals(object? obj)
        => obj is Entity<TId> other
           && GetType() == other.GetType()
           && EqualityComparer<TId>.Default.Equals(Id, other.Id);

    public override int GetHashCode()
        => HashCode.Combine(GetType(), Id);
}