using VEA.Core.Domain.Aggregates.GuestAggregate;
using VEA.Core.Domain.Aggregates.LocationAggregate;
using VEA.Core.Domain.Aggregates.VeaEventAggregate.ParticipationEntity;
using VEA.Core.Domain.Common.Bases;
using VEA.Core.Tools.OperationResult;
using VEA.Core.Tools.OperationResult.Errors;
using VEA.Core.Tools.OperationResult.Result;

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

    private VeaEvent() {} // EFC
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
        return EventTitle.Default
            .Map(title => new VeaEvent(
                id: EventId.New(),
                title: title,
                description: EventDescription.Default,
                timeRange: null,
                visibility: EventVisibility.Default,
                status: EventStatus.Default,
                location: null,
                guestCapacity: EventGuestCapacity.Default));
    }

    public Result<VeaEvent> UpdateTitle(EventTitle title)
    {
        var modificationError = GetGenericModificationError();

        if (modificationError is not null)
        {
            return new Failure<VeaEvent>([modificationError]);
        }

        Title = title;

        ResetStatusToDraftIfReady();

        return new Success<VeaEvent>(this);
    }

    public Result<VeaEvent> UpdateDescription(EventDescription description)
    {
        var modificationError = GetGenericModificationError();

        if (modificationError is not null)
        {
            return new Failure<VeaEvent>([modificationError]);
        }

        Description = description;

        ResetStatusToDraftIfReady();

        return new Success<VeaEvent>(this);
    }

    public Result<VeaEvent> UpdateTimeRange(EventTimeRange timeRange, DateTime currentTime)
    {
        var modificationError = GetGenericModificationError();

        if (modificationError is not null)
        {
            return new Failure<VeaEvent>([modificationError]);
        }

        if (timeRange.Start <= currentTime)
        {
            return new Failure<VeaEvent>([EventErrors.VeaEvent.EventTimeRangeMustBeInFuture]);
        }

        TimeRange = timeRange;

        ResetStatusToDraftIfReady();

        return new Success<VeaEvent>(this);
    }

    public Result<VeaEvent> SetVisibilityToPublic()
    {
        if (Status.Value == EventStatusValue.Cancelled)
        {
            return new Failure<VeaEvent>([EventErrors.VeaEvent.CancelledEventCannotBeModified]);
        }

        Visibility = EventVisibility.Public;

        return new Success<VeaEvent>(this);
    }

    public Result<VeaEvent> SetVisibilityToPrivate()
    {
        if (Status.Value == EventStatusValue.Cancelled)
        {
            return new Failure<VeaEvent>([EventErrors.VeaEvent.CancelledEventCannotBeModified]);
        }

        if (Status.Value == EventStatusValue.Active)
        {
            return new Failure<VeaEvent>([EventErrors.VeaEvent.ActiveEventCannotBeMadePrivate]);
        }

        Visibility = EventVisibility.Private;

        return new Success<VeaEvent>(this);
    }

    public Result<VeaEvent> SetGuestCapacity(EventGuestCapacity guestCapacity)
    {
        if (Status.Value == EventStatusValue.Cancelled)
        {
            return new Failure<VeaEvent>([EventErrors.VeaEvent.CancelledEventCannotBeModified]);
        }

        if (Status.Value == EventStatusValue.Active && guestCapacity.Value < GuestCapacity.Value)
        {
            return new Failure<VeaEvent>([EventErrors.VeaEvent.ActiveEventGuestCapacityCanOnlyIncrease]);
        }

        // TODO: Validate against location maximum guest capacity when/if Location is implemented.

        GuestCapacity = guestCapacity;

        ResetStatusToDraftIfReady();

        return new Success<VeaEvent>(this);
    }

    public Result<VeaEvent> MakeReady(DateTime currentTime)
    {
        if (Status.Value == EventStatusValue.Cancelled)
        {
            return new Failure<VeaEvent>([EventErrors.VeaEvent.CancelledEventCannotBeReadied]);
        }

        if (Status.Value == EventStatusValue.Active)
        {
            return new Failure<VeaEvent>([EventErrors.VeaEvent.ActiveEventCannotBeModified]);
        }

        var errors = ValidateReadyState(currentTime);

        if (errors.Count > 0)
        {
            return new Failure<VeaEvent>(errors);
        }

        Status = EventStatus.Ready;

        return new Success<VeaEvent>(this);
    }

    public Result<VeaEvent> MakeActive(DateTime currentTime)
    {
        return new Success<VeaEvent>(this)
            .Ensure(e => e.Status.Value != EventStatusValue.Cancelled,
                EventErrors.VeaEvent.CancelledEventCannotBeActivated)
            .Bind(e => e.Status.Value == EventStatusValue.Draft
                ? e.MakeReady(currentTime)
                : new Success<VeaEvent>(e))
            .Tap(e => e.Status = EventStatus.Active);
    }

    public Result<VeaEvent> Cancel()
    {
        if (Status.Value == EventStatusValue.Cancelled)
        {
            return new Success<VeaEvent>(this);
        }

        Status = EventStatus.Cancelled;

        return new Success<VeaEvent>(this);
    }

    private ResultError? GetGenericModificationError()
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

    private IReadOnlyList<ResultError> ValidateReadyState(DateTime currentTime)
    {
        var errors = new List<ResultError>();

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

        if (defaultTitleResult is Success<EventTitle> success)
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
        return new Success<VeaEvent>(this)
            .Ensure(e => e.Status.Value == EventStatusValue.Active,
                EventErrors.Participation.OnlyActiveEventsCanBeJoined)
            .Ensure(e => e.Visibility == EventVisibility.Public,
                EventErrors.Participation.OnlyPublicEventsCanBeParticipatedDirectly)
            .Ensure(e => e.TimeRange is not null && e.TimeRange.Start > now,
                EventErrors.Participation.OnlyFutureEventsCanBeParticipated)
            .Ensure(e => e.GetAttendingCount() < e.GuestCapacity.Value,
                EventErrors.Participation.EventCapacityReached)
            .Ensure(e => !e.HasParticipationForGuest(guestId),
                EventErrors.Participation.GuestAlreadyHasParticipation)
            .Bind(e => Participation.CreatePublicParticipation(guestId, e.Id, joinReason)
                .Map(p => { e._participations.Add(p); return e; }));
    }

    // UC12 Guest cancels participation
    public Result<VeaEvent> CancelParticipation(GuestId guestId, DateTime now)
    {
        var participation = FindParticipation(guestId);
        if (participation is null)
            return new Failure<VeaEvent>([EventErrors.Participation.ParticipationNotFound]);

        return new Success<Participation>(participation)
            .Ensure(p => !(Status.Value == EventStatusValue.Active &&
                           p.Status == ParticipationStatus.Attending &&
                           IsEventInPastOrOngoing(now)),
                EventErrors.Participation.CannotCancelParticipationInPastOrOngoingEvents)
            .Bind(p => p.CancelParticipation())
            .Map(p => { _participations.Remove(participation); return this; });
    }

    // UC13 Guest is invited to event
    public Result<VeaEvent> InviteGuest(
        GuestId guestId,
        ParticipationSource source,
        ParticipationJoinReason? joinReason = null)
    {
        return new Success<VeaEvent>(this)
            .Ensure(e => e.Status.Value == EventStatusValue.Ready || e.Status.Value == EventStatusValue.Active,
                EventErrors.Participation.GuestsCanOnlyBeInvitedToReadyOrActiveEvents)
            .Ensure(e => e.GetAttendingCount() < e.GuestCapacity.Value,
                EventErrors.Participation.CannotInviteToFullEvent)
            .Ensure(e => !e._participations.Any(p =>
                    p.GuestId == guestId &&
                    (p.Status == ParticipationStatus.Invited || p.Status == ParticipationStatus.Attending)),
                EventErrors.Participation.GuestAlreadyInvitedOrAttending)
            .Bind(e => Participation.CreateInvitation(guestId, source, e.Id, joinReason)
                .Map(p => { e._participations.Add(p); return e; }));
    }

    // UC14 Guest accepts invitation
    public Result<VeaEvent> AcceptInvitation(GuestId guestId, DateTime now)
    {
        return new Success<VeaEvent>(this)
            .Ensure(e => e.Status.Value == EventStatusValue.Active,
                EventErrors.Participation.EventMustBeActiveToAcceptInvitation)
            .Ensure(e => e.IsFutureEvent(now),
                EventErrors.Participation.OnlyFutureEventsCanBeParticipated)
            .Ensure(e => e.GetAttendingCount() < e.GuestCapacity.Value,
                EventErrors.Participation.EventCapacityReached)
            .Bind(e =>
            {
                var participation = e.FindParticipation(guestId, ParticipationStatus.Invited);
                if (participation is null)
                    return new Failure<VeaEvent>([EventErrors.Participation.ParticipationNotFound]);
                return participation.AcceptInvitation().Map(p => e);
            });
    }

    // UC15
    public Result<VeaEvent> DeclineInvitation(GuestId guestId)
    {
        return new Success<VeaEvent>(this)
            .Ensure(e => e.Status.Value == EventStatusValue.Active,
                EventErrors.Participation.EventMustBeActiveToDeclineInvitation)
            .Bind(e =>
            {
                var participation = e._participations.SingleOrDefault(p =>
                    p.GuestId == guestId &&
                    (p.Status == ParticipationStatus.Invited || p.Status == ParticipationStatus.Attending));
                if (participation is null)
                    return new Failure<VeaEvent>([EventErrors.Participation.ParticipationNotFound]);
                return participation.DeclineInvitation()
                    .Map(p => { e._participations.Remove(participation); return e; });
            });
    }
}