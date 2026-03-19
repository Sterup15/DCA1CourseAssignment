using VEA.Core.Domain.Aggregates.GuestAggregate;
using VEA.Core.Domain.Aggregates.VeaEventAggregate;
using VEA.Core.Domain.Aggregates.VeaEventAggregate.ParticipationEntity;
using VEA.Core.Tools.OperationResult;
using VEA.Core.Tools.OperationResult.Result;

namespace VEA.Core.Application.AppEntry.Commands.VeaEventCommands;

public record ParticipateEventAsGuestCommand
{
    public EventId EventId { get; }
    public GuestId GuestId { get; }
    public ParticipationJoinReason? JoinReason { get; }

    private ParticipateEventAsGuestCommand(EventId eventId, GuestId guestId, ParticipationJoinReason? joinReason)
    {
        EventId = eventId;
        GuestId = guestId;
        JoinReason = joinReason;
    }

    public static Result<ParticipateEventAsGuestCommand> Create(string eventId, string guestId, string? joinReason = null)
    {
        var eventIdResult = EventId.From(eventId);
        var guestIdResult = GuestId.From(guestId);

        if (joinReason is not null)
        {
            return ResultExtensions.CombineInto(
                eventIdResult,
                guestIdResult,
                ParticipationJoinReason.Create(joinReason),
                (eid, gid, jr) => new ParticipateEventAsGuestCommand(eid, gid, jr));
        }

        return ResultExtensions.CombineInto(
            eventIdResult,
            guestIdResult,
            (eid, gid) => new ParticipateEventAsGuestCommand(eid, gid, null));
    }
}
