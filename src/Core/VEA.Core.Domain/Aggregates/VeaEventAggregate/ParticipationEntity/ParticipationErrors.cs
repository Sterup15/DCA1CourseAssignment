using VEA.Core.Tools.OperationResult.Errors;

namespace VEA.Core.Domain.Aggregates.VeaEventAggregate.ParticipationEntity;

public static class ParticipationErrors
{
    public static class ParticipationId
    {
        private const string ParticipationIdCode = "Participation.ParticipationId";

        public static readonly ResultError Empty =
            new(ParticipationIdCode + ".empty", "ParticipationId cannot be empty.");
    }
    
    public static class ParticipationStatus
    {
        private const string ParticipationStatusCode = "Participation.Status";

        public static readonly ResultError Invalid =
            new(ParticipationStatusCode + ".invalid", "Participation status is invalid.");

        public static readonly ResultError AcceptRequiresInvited =
            new(ParticipationStatusCode + ".acceptRequiresInvited", "Only invited participation can be accepted.");

        public static readonly ResultError DeclineRequiresInvitedOrAttending =
            new(ParticipationStatusCode + ".declineRequiresInvitedOrAttending",
                "Only invited or attending participation can be declined.");

        public static readonly ResultError CancelRequiresInvitedOrAttending =
            new(ParticipationStatusCode + ".cancelRequiresInvitedOrAttending", "Only invited or attending participation can be cancelled.");
    }
    
    public static class ParticipationSource
    {
        private const string ParticipationSourceCode = "Participation.Source";

        public static readonly ResultError Invalid =
            new(ParticipationSourceCode + ".invalid", "Participation source is invalid.");
    }
    
    public static class ParticipationJoinReason
    {
        private const string Code = "Participation.JoinReason";

        public static readonly ResultError TooLong =
            new(Code + ".tooLong", "Participation join reason cannot exceed 100 characters.");
    }

}