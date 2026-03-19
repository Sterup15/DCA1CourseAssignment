using VEA.Core.Tools.OperationResult.Errors;

namespace VEA.Core.Domain.Aggregates.GuestAggregate;

public static class GuestErrors
{
    public static class GuestId
    {
        private const string GuestIdCode = "Guest.GuestId";

        public static readonly ResultError Empty =
            new(GuestIdCode + ".empty", "GuestId cannot be empty.");
    }
    
    public static class ViaMail
    {
        private const string Code = "Guest.ViaMail";

        public static readonly ResultError Empty =
            new(Code + ".empty", "ViaMail cannot be empty.");

        public static readonly ResultError InvalidDomain =
            new(Code + ".invalidDomain", "ViaMail must end with @via.dk.");

        public static readonly ResultError InvalidFormat =
            new(Code + ".invalidFormat",
                "ViaMail must be <text1>@via.dk where text1 is 3-4 letters or 6 digits.");

        public static readonly ResultError EmailInUse =
            new(Code + ".emailInUse", "An account with this email already exists.");
    }
    
    public static class Name
    {
        private const string Code = "Guest.Name";

        public static readonly ResultError Empty =
            new(Code + ".empty", "Name cannot be empty.");

        public static readonly ResultError TooShort =
            new(Code + ".tooShort", "Name must be at least 2 characters.");

        public static readonly ResultError TooLong =
            new(Code + ".tooLong", "Name cannot exceed 25 characters.");
        
        public static readonly ResultError InvalidCharacters =
            new(Code + ".invalidCharacters", "Name must contain only letters (a-z).");
    }
    
    public static class ProfilePicture
    {
        private const string Code = "Guest.ProfilePicture";

        public static readonly ResultError InvalidUrl =
            new(Code + ".invalidUrl", "Profile picture URL must be a valid absolute URL.");

        public static readonly ResultError InvalidScheme =
            new(Code + ".invalidScheme", "Profile picture URL must use HTTP or HTTPS.");
    }
}