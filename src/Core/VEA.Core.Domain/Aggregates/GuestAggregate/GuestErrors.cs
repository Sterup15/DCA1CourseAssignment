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
    
    public static class ViaMail
    {
        private const string Code = "Guest.ViaMail";

        public static readonly Error Empty =
            new(Code + ".empty", "ViaMail cannot be empty.");

        public static readonly Error InvalidDomain =
            new(Code + ".invalidDomain", "ViaMail must end with @via.dk.");

        public static readonly Error InvalidFormat =
            new(Code + ".invalidFormat",
                "ViaMail must be <text1>@via.dk where text1 is 3-4 letters or 6 digits.");
    }
    
    public static class Name
    {
        private const string Code = "Guest.Name";

        public static readonly Error Empty =
            new(Code + ".empty", "Name cannot be empty.");

        public static readonly Error TooShort =
            new(Code + ".tooShort", "Name must be at least 2 characters.");

        public static readonly Error TooLong =
            new(Code + ".tooLong", "Name cannot exceed 25 characters.");
        
        public static readonly Error InvalidCharacters =
            new(Code + ".invalidCharacters", "Name must contain only letters (a-z).");
    }
    
    public static class ProfilePicture
    {
        private const string Code = "Guest.ProfilePicture";

        public static readonly Error InvalidUrl =
            new(Code + ".invalidUrl", "Profile picture URL must be a valid absolute URL.");

        public static readonly Error InvalidScheme =
            new(Code + ".invalidScheme", "Profile picture URL must use HTTP or HTTPS.");
    }
}