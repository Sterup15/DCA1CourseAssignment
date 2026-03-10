using VEA.Core.Domain.Common.Bases;
using VEA.Core.Tools.OperationResult;
using VEA.Core.Tools.OperationResult.Errors;

namespace VEA.Core.Domain.Aggregates.GuestAggregate;

public sealed class Guest : AggregateRoot<GuestId>
{
    private static readonly Uri DefaultProfilePictureUrl = new(
        "https://media.istockphoto.com/id/521573873/vector/unknown-person-silhouette-whith-blue-tie.jpg?s=2048x2048&w=is&k=20&c=cjOrS4d7gV46uXDx9iWH5n5uSEF6hhZ6Gebbp5j6USI=");

    internal Name FirstName { get; private set; }
    internal Name LastName { get; private set; }
    internal ViaMail ViaMail { get; private set; }
    internal Uri ProfilePictureUrl { get; private set; }

    private Guest(
        GuestId id,
        Name firstName,
        Name lastName,
        ViaMail viaMail,
        Uri profilePictureUrl) : base(id)
    {
        FirstName = firstName;
        LastName = lastName;
        ViaMail = viaMail;
        ProfilePictureUrl = profilePictureUrl;
    }

    public static Result<Guest> Create(
        Name firstName,
        Name lastName,
        ViaMail email,
        Uri? profilePictureUrl = null)
    {
        var url = profilePictureUrl ?? DefaultProfilePictureUrl;

        var validationError = ValidateProfilePictureUrl(url);

        if (validationError is not null)
        {
            return Result<Guest>.Fail(validationError);
        }

        var guest = new Guest(
            id: GuestId.New(),
            firstName: firstName,
            lastName: lastName,
            viaMail: email,
            profilePictureUrl: url);

        return Result<Guest>.Ok(guest);
    }

    private static Error? ValidateProfilePictureUrl(Uri url)
    {
        if (!url.IsAbsoluteUri)
        {
            return GuestErrors.ProfilePicture.InvalidUrl;
        }

        if (url.Scheme != Uri.UriSchemeHttp && url.Scheme != Uri.UriSchemeHttps)
        {
            return GuestErrors.ProfilePicture.InvalidScheme;
        }

        return null;
    }
}