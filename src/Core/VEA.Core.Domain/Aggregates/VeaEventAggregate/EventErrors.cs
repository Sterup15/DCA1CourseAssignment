using VEA.Core.Tools.OperationResult.Errors;

namespace VEA.Core.Domain.Aggregates.VeaEventAggregate;

public static class EventErrors
{
    public static class VeaEvent
    {
        private const string EventCode = "Event";

        public static readonly Error ActiveEventCannotBeModified =
            new(EventCode + ".activeCannotBeModified", "Active event cannot be modified.");

        public static readonly Error CancelledEventCannotBeModified =
            new(EventCode + ".cancelledCannotBeModified", "Cancelled event cannot be modified.");

        public static readonly Error ActiveEventCannotBeMadePrivate =
            new(EventCode + ".activeCannotBeMadePrivate", "Active event cannot be made private.");

        public static readonly Error ActiveEventGuestCapacityCanOnlyIncrease =
            new(EventCode + ".activeGuestCapacityCanOnlyIncrease", "Maximum number of guests can only be increased for active events.");

        public static readonly Error CancelledEventCannotBeReadied =
            new(EventCode + ".cancelledCannotBeReadied", "Cancelled event cannot be readied.");

        public static readonly Error CancelledEventCannotBeActivated =
            new(EventCode + ".cancelledCannotBeActivated", "Cancelled event cannot be activated.");

        public static readonly Error EventTimeRangeMustBeInFuture =
            new(EventCode + ".timeRangeMustBeInFuture", "Event time range must be in the future.");

        public static readonly Error PastEventsCannotBeReadied =
            new(EventCode + ".pastEventsCannotBeReadied", "Past events cannot be readied.");

        public static readonly Error TitleMustNotBeDefault =
            new(EventCode + ".titleMustNotBeDefault", "Event title must not be the default title.");

        public static readonly Error DescriptionMustNotBeDefault =
            new(EventCode + ".descriptionMustNotBeDefault", "Event description must not be the default description.");

        public static readonly Error TimeRangeMustBeSet =
            new(EventCode + ".timeRangeMustBeSet", "Event time range must be set before the event can be readied.");
    }
    public static class EventId
    {
        private const string EventIdCode = "Event.EventId";
        
        public static readonly Error Empty =
            new(EventIdCode + ".empty", "EventId cannot be empty.");
    }
    
    public static class EventTitle
    {
        private const string EventTitleCode = "Event.EventTitle";

        public static readonly Error Empty =
            new(EventTitleCode + ".empty", "EventTitle cannot be empty.");

        public static readonly Error TooShort =
            new(EventTitleCode + ".tooShort", "EventTitle must be at least 3 characters.");

        public static readonly Error TooLong =
            new(EventTitleCode + ".tooLong", "EventTitle cannot exceed 75 characters.");
    }
    
    public static class EventDescription
    {
        private const string EventDescriptionCode = "Event.EventDescription";

        public static readonly Error TooLong =
            new(EventDescriptionCode + ".tooLong", "Event description cannot exceed 250 characters.");
    }
    
    public static class EventTimeRange
    {
        private const string EventTimeRangeCode = "Event.EventTimeRange";

        public static readonly Error EndMustBeAfterStart =
            new(EventTimeRangeCode + ".endMustBeAfterStart", "End time must be after start time.");

        public static readonly Error StartTooEarly =
            new(EventTimeRangeCode + ".startTooEarly", "Event cannot start before 08:00.");

        public static readonly Error EndOutsideAllowedHours =
            new(EventTimeRangeCode + ".endOutsideAllowedHours", "Event must end no later than 01:00 on the following day.");

        public static readonly Error TooLong =
            new(EventTimeRangeCode + ".tooLong", "Event time range cannot exceed 10 hours.");
        
        public static readonly Error TooShort =
            new(EventTimeRangeCode + ".tooShort", "Event time range must be at least 1 hour.");
        
        public static readonly Error StartEmpty =
            new(EventTimeRangeCode + ".startEmpty", "Event start time must be provided.");

        public static readonly Error EndEmpty =
            new(EventTimeRangeCode + ".endEmpty", "Event end time must be provided.");
    }
    
    public static class EventGuestCapacity
    {
        private const string EventGuestCapacityCode = "Event.EventGuestCapacity";

        public static readonly Error TooSmall =
            new(EventGuestCapacityCode + ".tooSmall", "Guest capacity must be at least 5.");

        public static readonly Error TooLarge =
            new(EventGuestCapacityCode + ".tooLarge", "Guest capacity cannot exceed 50.");
    }
    
    public static class EventStatus
    {
        private const string EventStatusCode = "Event.EventStatus";

        public static readonly Error Invalid =
            new(EventStatusCode + ".invalid", "Event status is invalid.");

        public static Error InvalidTransition(EventStatusValue from, EventStatusValue to) =>
            new(
                $"{EventStatusCode}.invalidTransition.{from}To{to}",
                $"Event status cannot transition from {from} to {to}.");
    }
    
    public static class EventVisibility
    {
        private const string EventVisibilityCode = "Event.EventVisibility";

        public static readonly Error Invalid =
            new(EventVisibilityCode + ".invalid", "Event visibility is invalid.");
    }
    
    public static class Participation
    {
        private const string ParticipationCode = "Event.Participation";

        public static readonly Error OnlyActiveEventsCanBeJoined =
            new(ParticipationCode + ".onlyActiveEventsCanBeJoined", "Only active events can be joined.");

        public static readonly Error EventCapacityReached =
            new(ParticipationCode + ".eventCapacityReached", "The event has reached its maximum guest capacity.");

        public static readonly Error OnlyFutureEventsCanBeParticipated =
            new(ParticipationCode + ".onlyFutureEventsCanBeParticipated", "Only future events can be participated in.");

        public static readonly Error OnlyPublicEventsCanBeParticipatedDirectly =
            new(ParticipationCode + ".onlyPublicEventsCanBeParticipatedDirectly", "Only public events can be participated in directly.");

        public static readonly Error GuestAlreadyHasParticipation =
            new(ParticipationCode + ".guestAlreadyHasParticipation", "Guest already has a participation for this event.");

        public static readonly Error GuestsCanOnlyBeInvitedToReadyOrActiveEvents =
            new(ParticipationCode + ".guestsCanOnlyBeInvitedToReadyOrActiveEvents", "Guests can only be invited to ready or active events.");

        public static readonly Error CannotInviteToFullEvent =
            new(ParticipationCode + ".cannotInviteToFullEvent", "Guests cannot be invited to a full event.");

        public static readonly Error GuestAlreadyInvitedOrAttending =
            new(ParticipationCode + ".guestAlreadyInvitedOrAttending", "Guest is already invited or attending this event.");

        public static readonly Error ParticipationNotFound =
            new(ParticipationCode + ".notFound", "Participation was not found for the guest and event.");

        public static readonly Error CannotCancelParticipationInPastOrOngoingEvents =
            new(ParticipationCode + ".cannotCancelPastOrOngoing", "Participation cannot be cancelled in past or ongoing events.");

        public static readonly Error EventMustBeActiveToAcceptInvitation =
            new(ParticipationCode + ".eventMustBeActiveToAcceptInvitation", "Invitation can only be accepted for active events.");

        public static readonly Error EventMustBeActiveToDeclineInvitation =
            new(ParticipationCode + ".eventMustBeActiveToDeclineInvitation", "Invitation can only be declined for active events.");
    }
}