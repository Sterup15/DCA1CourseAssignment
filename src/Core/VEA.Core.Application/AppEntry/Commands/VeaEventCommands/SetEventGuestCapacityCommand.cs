using VEA.Core.Domain.Aggregates.VeaEventAggregate;
using VEA.Core.Tools.OperationResult;
using VEA.Core.Tools.OperationResult.Result;

namespace VEA.Core.Application.AppEntry.Commands.VeaEventCommands;

public record SetEventGuestCapacityCommand
{
    public EventId EventId { get; }
    public EventGuestCapacity GuestCapacity { get; }

    private SetEventGuestCapacityCommand(EventId eventId, EventGuestCapacity guestCapacity)
    {
        EventId = eventId;
        GuestCapacity = guestCapacity;
    }

    public static Result<SetEventGuestCapacityCommand> Create(string eventId, int capacity)
        => ResultExtensions.CombineInto(
            EventId.From(eventId),
            EventGuestCapacity.Create(capacity),
            (id, cap) => new SetEventGuestCapacityCommand(id, cap));
}
