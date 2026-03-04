using VEA.Core.Tools.OperationResult.Errors;

namespace VEA.Core.Domain.Aggregates.VeaEventAggregate;

public static class EventErrors
{
    public static class EventId
    {
        private const string EventIdCode = "Event.EventId";
        
        public static readonly Error Empty =
            new(EventIdCode + ".empty", "EventId cannot be empty.");
    }
}