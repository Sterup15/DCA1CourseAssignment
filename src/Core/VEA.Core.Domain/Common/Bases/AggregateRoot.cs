namespace VEA.Core.Domain.Common.Bases;

public abstract class AggregateRoot<TId> : Entity<TId>
{
    protected AggregateRoot() { }
    protected AggregateRoot(TId id) : base(id) { }
}