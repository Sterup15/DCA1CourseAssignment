using VEA.Core.Domain.Aggregates.GuestAggregate;
using VEA.Core.Domain.Aggregates.VeaEventAggregate;
using VEA.Core.Tools.OperationResult;
using VEA.Core.Tools.OperationResult.Result;

namespace VEA.Core.Application.AppEntry.Commands.VeaEventCommands;

public record CancelEventParticipationCommand
{
    public EventId EventId { get; }
    public GuestId GuestId { get; }

    private CancelEventParticipationCommand(EventId eventId, GuestId guestId)
    {
        EventId = eventId;
        GuestId = guestId;
    }

    public static Result<CancelEventParticipationCommand> Create(string eventId, string guestId)
        => ResultExtensions.CombineInto(
            EventId.From(eventId),
            GuestId.From(guestId),
            (eid, gid) => new CancelEventParticipationCommand(eid, gid));
}
