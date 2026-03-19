using VEA.Core.Tools.OperationResult.Errors;

namespace VEA.Core.Domain.Aggregates.LocationAggregate;

public static class LocationErrors
{
    public static class LocationId
    {
        private const string LocationIdCode = "Location.LocationId";

        public static readonly ResultError Empty =
            new(LocationIdCode + ".empty", "LocationId cannot be empty.");
    }
}