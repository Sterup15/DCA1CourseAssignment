namespace UnitTests.Features.GuestAggregate;

using UnitTests.Fakes;
using VEA.Core.Domain.Aggregates.GuestAggregate;
using VEA.Core.Domain.Aggregates.GuestAggregate.Contracts;
using VEA.Core.Tools.OperationResult;
using Xunit;

public static class GuestTestFactory
{
    public static Name CreateName(string value)
    {
        var result = Name.Create(value);
        return Assert.IsType<Result<Name>.Success>(result).Value;
    }

    public static ViaMail CreateViaMail(string value, IEmailInUseChecker? checker = null)
    {
        var result = ViaMail.Create(value, checker ?? new FakeEmailInUseChecker());
        return Assert.IsType<Result<ViaMail>.Success>(result).Value;
    }

    public static Uri CreateValidProfilePictureUrl(string? value = null)
    {
        return new Uri(value ?? "https://example.com/profile.png");
    }

    public static Guest CreateGuest(
        string firstName = "john",
        string lastName = "doe",
        string email = "abc@via.dk",
        Uri? profilePictureUrl = null)
    {
        var result = Guest.Create(
            CreateName(firstName),
            CreateName(lastName),
            CreateViaMail(email),
            profilePictureUrl);

        return Assert.IsType<Result<Guest>.Success>(result).Value;
    }
}