using VEA.Core.Domain.Aggregates.GuestAggregate;
using VEA.Core.Tools.OperationResult;

namespace UnitTests.Features.GuestAggregate.RegisterGuest;

public class RegisterGuestAggregateTests
{
    [Theory]
    [InlineData("abc@via.dk", "joHN", "doE")]
    [InlineData("abcd@via.dk", "aNNa", "jEnSeN")]
    [InlineData("123456@via.dk", "maRk", "niElSeN")]
    public void Create_WhenInputIsValid_CreatesGuestAndNormalizesValues(
        string rawEmail,
        string rawFirstName,
        string rawLastName)
    {
        // Arrange
        var firstName = GuestTestFactory.CreateName(rawFirstName);
        var lastName = GuestTestFactory.CreateName(rawLastName);
        var viaMail = GuestTestFactory.CreateViaMail(rawEmail);
        var profilePictureUrl = GuestTestFactory.CreateValidProfilePictureUrl();

        // Act
        var result = Guest.Create(firstName, lastName, viaMail, profilePictureUrl);

        // Assert
        var success = Assert.IsType<Result<Guest>.Success>(result);
        var guest = success.Value;

        Assert.NotEqual(default, guest.Id);
        Assert.NotEqual(Guid.Empty, guest.Id.Value);

        var expectedFirstName =
            char.ToUpperInvariant(rawFirstName[0]) + rawFirstName[1..].ToLowerInvariant();

        var expectedLastName =
            char.ToUpperInvariant(rawLastName[0]) + rawLastName[1..].ToLowerInvariant();

        Assert.Equal(expectedFirstName, guest.FirstName.Value);
        Assert.Equal(expectedLastName, guest.LastName.Value);
        Assert.Equal(rawEmail.ToLowerInvariant(), guest.ViaMail.Value);
        Assert.Equal(profilePictureUrl, guest.ProfilePictureUrl);
    }

    [Fact]
    public void Create_WhenProfilePictureUrlIsNotProvided_UsesDefaultProfilePictureUrl()
    {
        // Arrange
        var firstName = GuestTestFactory.CreateName("john");
        var lastName = GuestTestFactory.CreateName("doe");
        var viaMail = GuestTestFactory.CreateViaMail("abc@via.dk");

        // Act
        var result = Guest.Create(firstName, lastName, viaMail);

        // Assert
        var success = Assert.IsType<Result<Guest>.Success>(result);
        var guest = success.Value;

        Assert.Equal(
            "https://media.istockphoto.com/id/521573873/vector/unknown-person-silhouette-whith-blue-tie.jpg?s=2048x2048&w=is&k=20&c=cjOrS4d7gV46uXDx9iWH5n5uSEF6hhZ6Gebbp5j6USI=",
            guest.ProfilePictureUrl.ToString());
    }

    [Theory]
    [InlineData("abc@gmail.com")]
    [InlineData("abcd@yahoo.dk")]
    [InlineData("123456@via.com")]
    public void CreateViaMail_WhenEmailDomainIsIncorrect_ReturnsFailure(string rawEmail)
    {
        // Act
        var result = ViaMail.Create(rawEmail);

        // Assert
        var failure = Assert.IsType<Result<ViaMail>.Failure>(result);
        Assert.Contains(failure.Errors, e => e == GuestErrors.ViaMail.InvalidDomain);
    }

    [Theory]
    [InlineData("ab@via.dk")]
    [InlineData("abcdef@via.dk")]
    [InlineData("abc12@via.dk")]
    [InlineData("abc@via")]
    [InlineData("abcvia.dk")]
    [InlineData("@via.dk")]
    public void CreateViaMail_WhenEmailFormatIsIncorrect_ReturnsFailure(string rawEmail)
    {
        // Act
        var result = ViaMail.Create(rawEmail);

        // Assert
        var failure = Assert.IsType<Result<ViaMail>.Failure>(result);
        Assert.True(
            failure.Errors.Contains(GuestErrors.ViaMail.InvalidFormat) ||
            failure.Errors.Contains(GuestErrors.ViaMail.InvalidDomain));
    }

    [Theory]
    [InlineData("abc@via.dk")]
    [InlineData("abcd@via.dk")]
    [InlineData("123456@via.dk")]
    public void CreateViaMail_WhenEmailIsAlreadyInUse_ReturnsFailure(string rawEmail)
    {
        // Act
        var result = ViaMail.Create(rawEmail, isEmailInUse: true);

        // Assert
        var failure = Assert.IsType<Result<ViaMail>.Failure>(result);
        Assert.Contains(failure.Errors, e => e == GuestErrors.ViaMail.EmailInUse);
    }

    [Theory]
    [InlineData("")]
    [InlineData("A")]
    [InlineData("A very very very very very long name")]
    public void CreateName_WhenFirstNameIsInvalid_ReturnsFailure(string rawFirstName)
    {
        // Act
        var result = Name.Create(rawFirstName);

        // Assert
        var failure = Assert.IsType<Result<Name>.Failure>(result);
        Assert.True(
            failure.Errors.Contains(GuestErrors.Name.Empty) ||
            failure.Errors.Contains(GuestErrors.Name.TooShort) ||
            failure.Errors.Contains(GuestErrors.Name.TooLong) ||
            failure.Errors.Contains(GuestErrors.Name.InvalidCharacters));
    }

    [Theory]
    [InlineData("")]
    [InlineData("B")]
    [InlineData("A very very very very very long surname")]
    public void CreateName_WhenLastNameIsInvalid_ReturnsFailure(string rawLastName)
    {
        // Act
        var result = Name.Create(rawLastName);

        // Assert
        var failure = Assert.IsType<Result<Name>.Failure>(result);
        Assert.True(
            failure.Errors.Contains(GuestErrors.Name.Empty) ||
            failure.Errors.Contains(GuestErrors.Name.TooShort) ||
            failure.Errors.Contains(GuestErrors.Name.TooLong) ||
            failure.Errors.Contains(GuestErrors.Name.InvalidCharacters));
    }

    [Fact]
    public void Create_WhenProfilePictureUrlIsRelative_ReturnsFailure()
    {
        // Arrange
        var firstName = GuestTestFactory.CreateName("john");
        var lastName = GuestTestFactory.CreateName("doe");
        var viaMail = GuestTestFactory.CreateViaMail("abc@via.dk");
        var relativeUrl = new Uri("/images/profile.png", UriKind.Relative);

        // Act
        var result = Guest.Create(firstName, lastName, viaMail, relativeUrl);

        // Assert
        var failure = Assert.IsType<Result<Guest>.Failure>(result);
        Assert.Contains(failure.Errors, e => e == GuestErrors.ProfilePicture.InvalidUrl);
    }

    [Theory]
    [InlineData("file:///C:/temp/profile.png")]
    [InlineData("ftp://example.com/profile.png")]
    public void Create_WhenProfilePictureUrlUsesInvalidScheme_ReturnsFailure(string rawUrl)
    {
        // Arrange
        var firstName = GuestTestFactory.CreateName("john");
        var lastName = GuestTestFactory.CreateName("doe");
        var viaMail = GuestTestFactory.CreateViaMail("abc@via.dk");
        var url = new Uri(rawUrl);

        // Act
        var result = Guest.Create(firstName, lastName, viaMail, url);

        // Assert
        var failure = Assert.IsType<Result<Guest>.Failure>(result);
        Assert.Contains(failure.Errors, e => e == GuestErrors.ProfilePicture.InvalidScheme);
    }
}