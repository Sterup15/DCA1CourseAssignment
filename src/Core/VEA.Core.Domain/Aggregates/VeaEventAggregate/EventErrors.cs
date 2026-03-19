using VEA.Core.Tools.OperationResult.Errors;

namespace VEA.Core.Domain.Aggregates.VeaEventAggregate;

public static class EventErrors
{
    public static class VeaEvent
    {
        private const string EventCode = "Event";

        public static readonly ResultError ActiveEventCannotBeModified =
            new(EventCode + ".activeCannotBeModified", "Active event cannot be modified.");

        public static readonly ResultError CancelledEventCannotBeModified =
            new(EventCode + ".cancelledCannotBeModified", "Cancelled event cannot be modified.");

        public static readonly ResultError ActiveEventCannotBeMadePrivate =
            new(EventCode + ".activeCannotBeMadePrivate", "Active event cannot be made private.");

        public static readonly ResultError ActiveEventGuestCapacityCanOnlyIncrease =
            new(EventCode + ".activeGuestCapacityCanOnlyIncrease", "Maximum number of guests can only be increased for active events.");

        public static readonly ResultError CancelledEventCannotBeReadied =
            new(EventCode + ".cancelledCannotBeReadied", "Cancelled event cannot be readied.");

        public static readonly ResultError CancelledEventCannotBeActivated =
            new(EventCode + ".cancelledCannotBeActivated", "Cancelled event cannot be activated.");

        public static readonly ResultError EventTimeRangeMustBeInFuture =
            new(EventCode + ".timeRangeMustBeInFuture", "Event time range must be in the future.");

        public static readonly ResultError PastEventsCannotBeReadied =
            new(EventCode + ".pastEventsCannotBeReadied", "Past events cannot be readied.");

        public static readonly ResultError TitleMustNotBeDefault =
            new(EventCode + ".titleMustNotBeDefault", "Event title must not be the default title.");

        public static readonly ResultError DescriptionMustNotBeDefault =
            new(EventCode + ".descriptionMustNotBeDefault", "Event description must not be the default description.");

        public static readonly ResultError TimeRangeMustBeSet =
            new(EventCode + ".timeRangeMustBeSet", "Event time range must be set before the event can be readied.");
        
        public static readonly ResultError EventNotFound =
            new(EventCode + ".eventNotFound", "Event object not found.");
    }
    public static class EventId
    {
        private const string EventIdCode = "Event.EventId";
        
        public static readonly ResultError Empty =
            new(EventIdCode + ".empty", "EventId cannot be empty.");

        public static readonly ResultError InvalidFormat =
            new(EventIdCode + ".invalidFormat", "EventId must be a valid GUID.");
    }
    
    public static class EventTitle
    {
        private const string EventTitleCode = "Event.EventTitle";

        public static readonly ResultError Empty =
            new(EventTitleCode + ".empty", "EventTitle cannot be empty.");

        public static readonly ResultError TooShort =
            new(EventTitleCode + ".tooShort", "EventTitle must be at least 3 characters.");

        public static readonly ResultError TooLong =
            new(EventTitleCode + ".tooLong", "EventTitle cannot exceed 75 characters.");
    }
    
    public static class EventDescription
    {
        private const string EventDescriptionCode = "Event.EventDescription";

        public static readonly ResultError TooLong =
            new(EventDescriptionCode + ".tooLong", "Event description cannot exceed 250 characters.");
    }
    
    public static class EventTimeRange
    {
        private const string EventTimeRangeCode = "Event.EventTimeRange";

        public static readonly ResultError EndMustBeAfterStart =
            new(EventTimeRangeCode + ".endMustBeAfterStart", "End time must be after start time.");

        public static readonly ResultError StartTooEarly =
            new(EventTimeRangeCode + ".startTooEarly", "Event cannot start before 08:00.");

        public static readonly ResultError EndOutsideAllowedHours =
            new(EventTimeRangeCode + ".endOutsideAllowedHours", "Event must end no later than 01:00 on the following day.");

        public static readonly ResultError TooLong =
            new(EventTimeRangeCode + ".tooLong", "Event time range cannot exceed 10 hours.");
        
        public static readonly ResultError TooShort =
            new(EventTimeRangeCode + ".tooShort", "Event time range must be at least 1 hour.");
        
        public static readonly ResultError StartEmpty =
            new(EventTimeRangeCode + ".startEmpty", "Event start time must be provided.");

        public static readonly ResultError EndEmpty =
            new(EventTimeRangeCode + ".endEmpty", "Event end time must be provided.");
    }
    
    public static class EventGuestCapacity
    {
        private const string EventGuestCapacityCode = "Event.EventGuestCapacity";

        public static readonly ResultError TooSmall =
            new(EventGuestCapacityCode + ".tooSmall", "Guest capacity must be at least 5.");

        public static readonly ResultError TooLarge =
            new(EventGuestCapacityCode + ".tooLarge", "Guest capacity cannot exceed 50.");
    }
    
    public static class EventStatus
    {
        private const string EventStatusCode = "Event.EventStatus";

        public static readonly ResultError Invalid =
            new(EventStatusCode + ".invalid", "Event status is invalid.");

        public static ResultError InvalidTransition(EventStatusValue from, EventStatusValue to) =>
            new(
                $"{EventStatusCode}.invalidTransition.{from}To{to}",
                $"Event status cannot transition from {from} to {to}.");
    }
    
    public static class EventVisibility
    {
        private const string EventVisibilityCode = "Event.EventVisibility";

        public static readonly ResultError Invalid =
            new(EventVisibilityCode + ".invalid", "Event visibility is invalid.");
    }
    
    public static class Participation
    {
        private const string ParticipationCode = "Event.Participation";

        public static readonly ResultError OnlyActiveEventsCanBeJoined =
            new(ParticipationCode + ".onlyActiveEventsCanBeJoined", "Only active events can be joined.");

        public static readonly ResultError EventCapacityReached =
            new(ParticipationCode + ".eventCapacityReached", "The event has reached its maximum guest capacity.");

        public static readonly ResultError OnlyFutureEventsCanBeParticipated =
            new(ParticipationCode + ".onlyFutureEventsCanBeParticipated", "Only future events can be participated in.");

        public static readonly ResultError OnlyPublicEventsCanBeParticipatedDirectly =
            new(ParticipationCode + ".onlyPublicEventsCanBeParticipatedDirectly", "Only public events can be participated in directly.");

        public static readonly ResultError GuestAlreadyHasParticipation =
            new(ParticipationCode + ".guestAlreadyHasParticipation", "Guest already has a participation for this event.");

        public static readonly ResultError GuestsCanOnlyBeInvitedToReadyOrActiveEvents =
            new(ParticipationCode + ".guestsCanOnlyBeInvitedToReadyOrActiveEvents", "Guests can only be invited to ready or active events.");

        public static readonly ResultError CannotInviteToFullEvent =
            new(ParticipationCode + ".cannotInviteToFullEvent", "Guests cannot be invited to a full event.");

        public static readonly ResultError GuestAlreadyInvitedOrAttending =
            new(ParticipationCode + ".guestAlreadyInvitedOrAttending", "Guest is already invited or attending this event.");

        public static readonly ResultError ParticipationNotFound =
            new(ParticipationCode + ".notFound", "Participation was not found for the guest and event.");

        public static readonly ResultError CannotCancelParticipationInPastOrOngoingEvents =
            new(ParticipationCode + ".cannotCancelPastOrOngoing", "Participation cannot be cancelled in past or ongoing events.");

        public static readonly ResultError EventMustBeActiveToAcceptInvitation =
            new(ParticipationCode + ".eventMustBeActiveToAcceptInvitation", "Invitation can only be accepted for active events.");

        public static readonly ResultError EventMustBeActiveToDeclineInvitation =
            new(ParticipationCode + ".eventMustBeActiveToDeclineInvitation", "Invitation can only be declined for active events.");
    }
}