using VEA.Core.Tools.OperationResult.Errors;

namespace VEA.Core.Domain.Aggregates.GuestAggregate;

public static class GuestErrors
{
    public static class GuestId
    {
        private const string GuestIdCode = "Guest.GuestId";

        public static readonly Error Empty =
            new(GuestIdCode + ".empty", "GuestId cannot be empty.");
    }
}