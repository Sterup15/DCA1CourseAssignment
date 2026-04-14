using VEA.Core.Domain.Aggregates.GuestAggregate;
using VEA.Core.Tools.OperationResult.Result;

namespace IntegrationTests.Common;

public static class GuestTestFactory
{
    public static Guest CreateGuest(
        string firstName = "John",
        string lastName = "Doe",
        string email = "abc@via.dk",
        Uri? profilePictureUrl = null)
    {
        var firstNameVo = Assert.IsType<Success<Name>>(Name.Create(firstName)).Value;
        var lastNameVo = Assert.IsType<Success<Name>>(Name.Create(lastName)).Value;
        var emailVo = Assert.IsType<Success<ViaMail>>(ViaMail.Create(email)).Value;

        var result = Guest.Create(firstNameVo, lastNameVo, emailVo, profilePictureUrl);
        return Assert.IsType<Success<Guest>>(result).Value;
    }
}
