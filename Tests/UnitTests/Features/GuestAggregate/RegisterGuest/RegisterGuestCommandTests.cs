using VEA.Core.Application.AppEntry.Commands.GuestCommands;
using VEA.Core.Domain.Aggregates.GuestAggregate;
using VEA.Core.Tools.OperationResult;
using VEA.Core.Tools.OperationResult.Result;

namespace UnitTests.Features.GuestAggregate.RegisterGuest;

public class RegisterGuestCommandTests
{
    [Fact]
    public void Create_WithValidInputs_ReturnsSuccess()
    {
        // Act
        var result = RegisterGuestCommand.Create("John", "Doe", "abc@via.dk");

        // Assert
        Assert.IsType<Success<RegisterGuestCommand>>(result);
    }

    [Fact]
    public void Create_WithEmptyFirstName_ReturnsFailure()
    {
        // Act
        var result = RegisterGuestCommand.Create("", "Doe", "abc@via.dk");

        // Assert
        var failure = Assert.IsType<Failure<RegisterGuestCommand>>(result);
        Assert.Contains(failure.Errors, e => e == GuestErrors.Name.Empty);
    }
}
