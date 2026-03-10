using VEA.Core.Domain.Aggregates.GuestAggregate;
using VEA.Core.Domain.Aggregates.LocationAggregate;
using VEA.Core.Domain.Aggregates.VeaEventAggregate.ParticipationEntity;
using VEA.Core.Domain.Common.Bases;
using VEA.Core.Tools.OperationResult;
using VEA.Core.Tools.OperationResult.Errors;

namespace VEA.Core.Domain.Aggregates.VeaEventAggregate;

public sealed class VeaEvent : AggregateRoot<EventId>
{
    private readonly List<Participation> _participations = [];

    internal EventTitle Title { get; private set; }
    internal EventDescription Description { get; private set; }
    internal EventTimeRange? TimeRange { get; private set; }
    internal EventVisibility Visibility { get; private set; }
    internal EventStatus Status { get; private set; }
    internal LocationId? Location { get; private set; }
    internal IReadOnlyCollection<Participation> Participations => _participations.AsReadOnly();
    internal EventGuestCapacity GuestCapacity { get; private set; }

    private VeaEvent(
        EventId id,
        EventTitle title,
        EventDescription description,
        EventTimeRange? timeRange,
        EventVisibility visibility,
        EventStatus status,
        LocationId? location,
        EventGuestCapacity guestCapacity) : base(id)
    {
        Title = title;
        Description = description;
        TimeRange = timeRange;
        Visibility = visibility;
        Status = status;
        Location = location;
        GuestCapacity = guestCapacity;
    }

    public static Result<VeaEvent> Create()
    {
        var errors = new List<Error>();

        var titleResult = EventTitle.Default;

        if (titleResult is Result<EventTitle>.Failure titleFailure)
        {
            errors.AddRange(titleFailure.Errors);
        }

        if (errors.Count > 0)
        {
            return Result<VeaEvent>.Fail(errors.ToArray());
        }

        var title = ((Result<EventTitle>.Success)titleResult).Value;

        var veaEvent = new VeaEvent(
            id: EventId.New(),
            title: title,
            description: EventDescription.Default,
            timeRange: null,
            visibility: EventVisibility.Default,
            status: EventStatus.Default,
            location: null,
            guestCapacity: EventGuestCapacity.Default);

        return Result<VeaEvent>.Ok(veaEvent);
    }

    public Result<VeaEvent> UpdateTitle(EventTitle title)
    {
        var modificationError = GetGenericModificationError();

        if (modificationError is not null)
        {
            return Result<VeaEvent>.Fail(modificationError);
        }

        Title = title;

        ResetStatusToDraftIfReady();

        return Result<VeaEvent>.Ok(this);
    }

    public Result<VeaEvent> UpdateDescription(EventDescription description)
    {
        var modificationError = GetGenericModificationError();

        if (modificationError is not null)
        {
            return Result<VeaEvent>.Fail(modificationError);
        }

        Description = description;

        ResetStatusToDraftIfReady();

        return Result<VeaEvent>.Ok(this);
    }

    public Result<VeaEvent> UpdateTimeRange(EventTimeRange timeRange, DateTime currentTime)
    {
        var modificationError = GetGenericModificationError();

        if (modificationError is not null)
        {
            return Result<VeaEvent>.Fail(modificationError);
        }

        if (timeRange.Start <= currentTime)
        {
            return Result<VeaEvent>.Fail(EventErrors.VeaEvent.EventTimeRangeMustBeInFuture);
        }

        TimeRange = timeRange;

        ResetStatusToDraftIfReady();

        return Result<VeaEvent>.Ok(this);
    }

    public Result<VeaEvent> SetVisibilityToPublic()
    {
        if (Status.Value == EventStatusValue.Cancelled)
        {
            return Result<VeaEvent>.Fail(EventErrors.VeaEvent.CancelledEventCannotBeModified);
        }

        Visibility = EventVisibility.Public;

        return Result<VeaEvent>.Ok(this);
    }

    public Result<VeaEvent> SetVisibilityToPrivate()
    {
        if (Status.Value == EventStatusValue.Cancelled)
        {
            return Result<VeaEvent>.Fail(EventErrors.VeaEvent.CancelledEventCannotBeModified);
        }

        if (Status.Value == EventStatusValue.Active)
        {
            return Result<VeaEvent>.Fail(EventErrors.VeaEvent.ActiveEventCannotBeMadePrivate);
        }

        Visibility = EventVisibility.Private;

        return Result<VeaEvent>.Ok(this);
    }

    public Result<VeaEvent> SetGuestCapacity(EventGuestCapacity guestCapacity)
    {
        if (Status.Value == EventStatusValue.Cancelled)
        {
            return Result<VeaEvent>.Fail(EventErrors.VeaEvent.CancelledEventCannotBeModified);
        }

        if (Status.Value == EventStatusValue.Active && guestCapacity.Value < GuestCapacity.Value)
        {
            return Result<VeaEvent>.Fail(EventErrors.VeaEvent.ActiveEventGuestCapacityCanOnlyIncrease);
        }

        // TODO: Validate against location maximum guest capacity when/if Location is implemented.

        GuestCapacity = guestCapacity;

        ResetStatusToDraftIfReady();

        return Result<VeaEvent>.Ok(this);
    }

    public Result<VeaEvent> MakeReady(DateTime currentTime)
    {
        if (Status.Value == EventStatusValue.Cancelled)
        {
            return Result<VeaEvent>.Fail(EventErrors.VeaEvent.CancelledEventCannotBeReadied);
        }

        if (Status.Value == EventStatusValue.Active)
        {
            return Result<VeaEvent>.Fail(EventErrors.VeaEvent.ActiveEventCannotBeModified);
        }

        var errors = ValidateReadyState(currentTime);

        if (errors.Count > 0)
        {
            return Result<VeaEvent>.Fail(errors.ToArray());
        }

        Status = EventStatus.Ready;

        return Result<VeaEvent>.Ok(this);
    }

    public Result<VeaEvent> MakeActive(DateTime currentTime)
    {
        if (Status.Value == EventStatusValue.Cancelled)
        {
            return Result<VeaEvent>.Fail(EventErrors.VeaEvent.CancelledEventCannotBeActivated);
        }

        if (Status.Value == EventStatusValue.Active)
        {
            return Result<VeaEvent>.Ok(this);
        }

        if (Status.Value == EventStatusValue.Draft)
        {
            var readyResult = MakeReady(currentTime);

            if (readyResult is Result<VeaEvent>.Failure readyFailure)
            {
                return Result<VeaEvent>.Fail(readyFailure.Errors.ToArray());
            }
        }

        Status = EventStatus.Active;

        return Result<VeaEvent>.Ok(this);
    }
    
    public Result<VeaEvent> Cancel()
    {
        if (Status.Value == EventStatusValue.Cancelled)
        {
            return Result<VeaEvent>.Ok(this);
        }

        Status = EventStatus.Cancelled;

        return Result<VeaEvent>.Ok(this);
    }

    private Error? GetGenericModificationError()
    {
        return Status.Value switch
        {
            EventStatusValue.Active => EventErrors.VeaEvent.ActiveEventCannotBeModified,
            EventStatusValue.Cancelled => EventErrors.VeaEvent.CancelledEventCannotBeModified,
            _ => null
        };
    }

    private void ResetStatusToDraftIfReady()
    {
        if (Status.Value == EventStatusValue.Ready)
        {
            Status = EventStatus.Draft;
        }
    }

    private IReadOnlyList<Error> ValidateReadyState(DateTime currentTime)
    {
        var errors = new List<Error>();

        if (IsDefaultTitle())
        {
            errors.Add(EventErrors.VeaEvent.TitleMustNotBeDefault);
        }

        if (IsDefaultDescription())
        {
            errors.Add(EventErrors.VeaEvent.DescriptionMustNotBeDefault);
        }

        if (TimeRange is null)
        {
            errors.Add(EventErrors.VeaEvent.TimeRangeMustBeSet);
        }
        else if (TimeRange.Start <= currentTime)
        {
            errors.Add(EventErrors.VeaEvent.PastEventsCannotBeReadied);
        }

        return errors;
    }

    // Slightly awkward due to having validation example for this default, keeping it in as example for both scenarios.
    private bool IsDefaultTitle()
    {
        var defaultTitleResult = EventTitle.Default;

        if (defaultTitleResult is Result<EventTitle>.Success success)
        {
            return Title == success.Value;
        }

        return false;
    }
    
    private static bool IsDefaultDescriptionValue(EventDescription description) =>
        description == EventDescription.Default;

    private bool IsDefaultDescription() => IsDefaultDescriptionValue(Description);
    
    // Participation events
    
    // Helpers
    
    private bool HasParticipationForGuest(GuestId guestId) =>
        _participations.Any(p => p.GuestId == guestId);

    private int GetAttendingCount() =>
        _participations.Count(p => p.Status == ParticipationStatus.Attending);

    private Participation? FindParticipation(GuestId guestId) =>
        _participations.SingleOrDefault(p => p.GuestId == guestId);

    private Participation? FindParticipation(GuestId guestId, ParticipationStatus status) =>
        _participations.SingleOrDefault(p => p.GuestId == guestId && p.Status == status);

    private bool IsEventInPastOrOngoing(DateTime now) =>
        TimeRange is not null && TimeRange.Start <= now;

    private bool IsFutureEvent(DateTime now) =>
        TimeRange is not null && TimeRange.Start > now;

    // UC11 Guest participates public event
    public Result<VeaEvent> ParticipateAsGuest(
        GuestId guestId,
        DateTime now,
        ParticipationJoinReason? joinReason = null)
    {
        if (Status.Value != EventStatusValue.Active)
        {
            return Result<VeaEvent>.Fail(EventErrors.Participation.OnlyActiveEventsCanBeJoined);
        }

        if (Visibility != EventVisibility.Public)
        {
            return Result<VeaEvent>.Fail(EventErrors.Participation.OnlyPublicEventsCanBeParticipatedDirectly);
        }

        if (TimeRange is null || TimeRange.Start <= now)
        {
            return Result<VeaEvent>.Fail(EventErrors.Participation.OnlyFutureEventsCanBeParticipated);
        }

        if (GetAttendingCount() >= GuestCapacity.Value)
        {
            return Result<VeaEvent>.Fail(EventErrors.Participation.EventCapacityReached);
        }

        if (HasParticipationForGuest(guestId))
        {
            return Result<VeaEvent>.Fail(EventErrors.Participation.GuestAlreadyHasParticipation);
        }

        var participationResult = Participation.CreatePublicParticipation(
            guestId,
            Id,
            joinReason);

        if (participationResult is Result<Participation>.Failure failure)
        {
            return Result<VeaEvent>.Fail(failure.Errors.ToArray());
        }

        var participation = ((Result<Participation>.Success)participationResult).Value;
        _participations.Add(participation);

        return Result<VeaEvent>.Ok(this);
    }
    
    // UC12 Guest cancels participation
    public Result<VeaEvent> CancelParticipation(GuestId guestId, DateTime now)
    {
        var participation = FindParticipation(guestId);

        if (participation is null)
        {
            return Result<VeaEvent>.Fail(EventErrors.Participation.ParticipationNotFound);
        }

        if (Status.Value == EventStatusValue.Active &&
            participation.Status == ParticipationStatus.Attending &&
            TimeRange is not null &&
            TimeRange.Start <= now)
        {
            return Result<VeaEvent>.Fail(EventErrors.Participation.CannotCancelParticipationInPastOrOngoingEvents);
        }

        var cancelResult = participation.CancelParticipation();

        if (cancelResult is Result<Participation>.Failure failure)
        {
            return Result<VeaEvent>.Fail(failure.Errors.ToArray());
        }

        _participations.Remove(participation);

        return Result<VeaEvent>.Ok(this);
    }
    
    // UC13 Guest is invited to event
    public Result<VeaEvent> InviteGuest(
        GuestId guestId,
        ParticipationSource source,
        ParticipationJoinReason? joinReason = null)
    {
        if (Status.Value != EventStatusValue.Ready &&
            Status.Value != EventStatusValue.Active)
        {
            return Result<VeaEvent>.Fail(EventErrors.Participation.GuestsCanOnlyBeInvitedToReadyOrActiveEvents);
        }

        if (GetAttendingCount() >= GuestCapacity.Value)
        {
            return Result<VeaEvent>.Fail(EventErrors.Participation.CannotInviteToFullEvent);
        }

        if (_participations.Any(p =>
                p.GuestId == guestId &&
                (p.Status == ParticipationStatus.Invited || p.Status == ParticipationStatus.Attending)))
        {
            return Result<VeaEvent>.Fail(EventErrors.Participation.GuestAlreadyInvitedOrAttending);
        }

        var participationResult = Participation.CreateInvitation(
            guestId,
            source,
            Id,
            joinReason);

        if (participationResult is Result<Participation>.Failure failure)
        {
            return Result<VeaEvent>.Fail(failure.Errors.ToArray());
        }

        var participation = ((Result<Participation>.Success)participationResult).Value;
        _participations.Add(participation);

        return Result<VeaEvent>.Ok(this);
    }
    
    // UC14 Guest accepts invitation
    public Result<VeaEvent> AcceptInvitation(GuestId guestId, DateTime now)
    {
        if (Status.Value != EventStatusValue.Active)
        {
            return Result<VeaEvent>.Fail(EventErrors.Participation.EventMustBeActiveToAcceptInvitation);
        }

        if (TimeRange is null || TimeRange.Start <= now)
        {
            return Result<VeaEvent>.Fail(EventErrors.Participation.OnlyFutureEventsCanBeParticipated);
        }

        if (GetAttendingCount() >= GuestCapacity.Value)
        {
            return Result<VeaEvent>.Fail(EventErrors.Participation.EventCapacityReached);
        }

        var participation = FindParticipation(guestId, ParticipationStatus.Invited);

        if (participation is null)
        {
            return Result<VeaEvent>.Fail(EventErrors.Participation.ParticipationNotFound);
        }

        var acceptResult = participation.AcceptInvitation();

        if (acceptResult is Result<Participation>.Failure failure)
        {
            return Result<VeaEvent>.Fail(failure.Errors.ToArray());
        }

        return Result<VeaEvent>.Ok(this);
    }
    
    // UC15
    public Result<VeaEvent> DeclineInvitation(GuestId guestId)
    {
        if (Status.Value != EventStatusValue.Active)
        {
            return Result<VeaEvent>.Fail(EventErrors.Participation.EventMustBeActiveToDeclineInvitation);
        }

        var participation = _participations
            .SingleOrDefault(p =>
                p.GuestId == guestId &&
                (p.Status == ParticipationStatus.Invited ||
                 p.Status == ParticipationStatus.Attending));

        if (participation is null)
        {
            return Result<VeaEvent>.Fail(EventErrors.Participation.ParticipationNotFound);
        }

        var declineResult = participation.DeclineInvitation();

        if (declineResult is Result<Participation>.Failure failure)
        {
            return Result<VeaEvent>.Fail(failure.Errors.ToArray());
        }

        _participations.Remove(participation);

        return Result<VeaEvent>.Ok(this);
    }
}