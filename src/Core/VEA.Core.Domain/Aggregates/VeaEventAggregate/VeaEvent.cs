using VEA.Core.Domain.Common.Bases;
using VEA.Core.Tools.OperationResult;

namespace VEA.Core.Domain.Aggregates.VeaEventAggregate;

public class VeaEvent : AggregateRoot<EventId>
{
    private VeaEvent(EventId id)
    {
        Id = id;
    }
    public static Result<VeaEvent> Create(EventId id)
    {
        
    }
}