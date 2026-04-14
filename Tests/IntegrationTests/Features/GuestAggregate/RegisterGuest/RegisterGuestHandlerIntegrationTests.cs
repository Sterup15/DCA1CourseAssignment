using IntegrationTests.Common;
using VEA.Core.Application.AppEntry.Commands.GuestCommands;
using VEA.Core.Application.Features.GuestHandlers.RegisterGuest;
using VEA.Core.Domain.Aggregates.GuestAggregate;
using VEA.Core.Tools.OperationResult.Result;

namespace IntegrationTests.Features.GuestAggregate.RegisterGuest;

public class RegisterGuestHandlerIntegrationTests : IntegrationTestBase
{
    [Fact]
    public async Task HandleAsync_WithValidCommand_CreatesGuestAndPersists()
    {
        // Arrange
        var command = Assert.IsType<Success<RegisterGuestCommand>>(
            RegisterGuestCommand.Create("John", "Doe", "abc@via.dk")).Value;
        var handler = new RegisterGuestHandler(GuestRepository, UnitOfWork);

        // Act
        var result = await handler.HandleAsync(command);

        // Assert
        var success = Assert.IsType<Success<GuestId>>(result);
        Assert.NotEqual(Guid.Empty, success.Value.Value);

        await using var fresh = CreateFreshContext();
        var savedGuest = await fresh.Guests.FindAsync(success.Value);
        Assert.NotNull(savedGuest);
    }

    [Fact]
    public void Create_WithInvalidEmail_ReturnsFailure()
    {
        // Act
        var result = RegisterGuestCommand.Create("John", "Doe", "not-an-email");

        // Assert
        var failure = Assert.IsType<Failure<RegisterGuestCommand>>(result);
        Assert.NotEmpty(failure.Errors);
    }
}
