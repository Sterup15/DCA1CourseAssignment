using VEA.Core.Domain.Aggregates.GuestAggregate;
using VEA.Core.Domain.Common.Bases;
using VEA.Core.Tools.OperationResult;
using VEA.Core.Tools.OperationResult.Result;

namespace VEA.Core.Domain.Aggregates.VeaEventAggregate.ParticipationEntity;

public sealed class Participation : Entity<ParticipationId>
{
    internal ParticipationStatus Status { get; private set; }
    internal ParticipationJoinReason JoinReason { get; private set; }
    internal GuestId GuestId { get; private set; }
    internal ParticipationSource Source { get; private set; }
    internal EventId EventId { get; private set; }

    private Participation(
        ParticipationId id,
        ParticipationStatus status,
        ParticipationJoinReason joinReason,
        GuestId guestId,
        ParticipationSource source,
        EventId eventId) : base(id)
    {
        Status = status;
        JoinReason = joinReason;
        GuestId = guestId;
        Source = source;
        EventId = eventId;
    }

    internal static Result<Participation> CreatePublicParticipation(
        GuestId guestId,
        EventId eventId,
        ParticipationJoinReason? joinReason = null)
    {
        var participation = new Participation(
            id: ParticipationId.New(),
            status: ParticipationStatus.Attending,
            joinReason: joinReason ?? ParticipationJoinReason.Default,
            guestId: guestId,
            source: ParticipationSource.Public,
            eventId: eventId);

        return new Success<Participation>(participation);
    }

    internal static Result<Participation> CreateInvitation(
        GuestId guestId,
        ParticipationSource source,
        EventId eventId,
        ParticipationJoinReason? joinReason = null)
    {
        var participation = new Participation(
            id: ParticipationId.New(),
            status: ParticipationStatus.Invited,
            joinReason: joinReason ?? ParticipationJoinReason.Default,
            guestId: guestId,
            source: source,
            eventId: eventId);

        return new Success<Participation>(participation);
    }

    internal Result<Participation> AcceptInvitation()
    {
        if (Status != ParticipationStatus.Invited)
        {
            return new Failure<Participation>([ParticipationErrors.ParticipationStatus.AcceptRequiresInvited]);
        }

        Status = ParticipationStatus.Attending;

        return new Success<Participation>(this);
    }

    internal Result<Participation> DeclineInvitation()
    {
        if (Status != ParticipationStatus.Invited &&
            Status != ParticipationStatus.Attending)
        {
            return new Failure<Participation>([ParticipationErrors.ParticipationStatus.DeclineRequiresInvitedOrAttending]);
        }

        Status = ParticipationStatus.Rejected;

        return new Success<Participation>(this);
    }

    internal Result<Participation> CancelParticipation()
    {
        if (Status != ParticipationStatus.Attending &&
            Status != ParticipationStatus.Invited)
        {
            return new Failure<Participation>([ParticipationErrors.ParticipationStatus.CancelRequiresInvitedOrAttending]);
        }

        Status = ParticipationStatus.Rejected;

        return new Success<Participation>(this);
    }
}