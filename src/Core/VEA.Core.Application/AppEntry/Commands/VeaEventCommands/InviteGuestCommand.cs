using VEA.Core.Domain.Aggregates.GuestAggregate;
using VEA.Core.Domain.Aggregates.VeaEventAggregate;
using VEA.Core.Domain.Aggregates.VeaEventAggregate.ParticipationEntity;
using VEA.Core.Tools.OperationResult;
using VEA.Core.Tools.OperationResult.Result;

namespace VEA.Core.Application.AppEntry.Commands.VeaEventCommands;

public record InviteGuestCommand
{
    public EventId EventId { get; }
    public GuestId GuestId { get; }
    public ParticipationSource Source { get; }
    public ParticipationJoinReason? JoinReason { get; }

    private InviteGuestCommand(EventId eventId, GuestId guestId, ParticipationSource source, ParticipationJoinReason? joinReason)
    {
        EventId = eventId;
        GuestId = guestId;
        Source = source;
        JoinReason = joinReason;
    }

    public static Result<InviteGuestCommand> Create(string eventId, string guestId, ParticipationSourceValue source, string? joinReason = null)
    {
        var eventIdResult = EventId.From(eventId);
        var guestIdResult = GuestId.From(guestId);
        var sourceResult = ParticipationSource.Create(source);

        if (joinReason is not null)
        {
            return ResultExtensions.CombineInto(
                    eventIdResult,
                    guestIdResult,
                    sourceResult,
                    (eid, gid, src) => (eid, gid, src))
                .Bind(t => ParticipationJoinReason.Create(joinReason)
                    .Map(jr => new InviteGuestCommand(t.eid, t.gid, t.src, jr)));
        }

        return ResultExtensions.CombineInto(
            eventIdResult,
            guestIdResult,
            sourceResult,
            (eid, gid, src) => new InviteGuestCommand(eid, gid, src, null));
    }
}
