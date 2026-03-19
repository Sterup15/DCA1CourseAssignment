using VEA.Core.Tools.OperationResult.Result;

namespace UnitTests.Features.GuestAggregate;

using VEA.Core.Domain.Aggregates.GuestAggregate;
using VEA.Core.Tools.OperationResult;
using Xunit;

public static class GuestTestFactory
{
    public static Name CreateName(string value)
    {
        var result = Name.Create(value);
        return Assert.IsType<Success<Name>>(result).Value;
    }

    public static ViaMail CreateViaMail(string value)
    {
        var result = ViaMail.Create(value);
        return Assert.IsType<Success<ViaMail>>(result).Value;
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

        return Assert.IsType<Success<Guest>>(result).Value;
    }
}