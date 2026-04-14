using IntegrationTests.Common;
using VEA.Core.Application.AppEntry.Commands.VeaEventCommands;
using VEA.Core.Application.Features.VeaEventHandlers.MakeEventPublic;
using VEA.Core.Domain.Aggregates.VeaEventAggregate;
using VEA.Core.Tools.OperationResult;
using VEA.Core.Tools.OperationResult.Result;

namespace IntegrationTests.Features.EventAggregate.MakeEventPublic;

public class MakeEventPublicHandlerIntegrationTests : IntegrationTestBase
{
    [Fact]
    public async Task HandleAsync_WhenEventExists_MakesEventPublicAndPersists()
    {
        // Arrange
        var veaEvent = VeaEventTestFactory.CreateEvent();
        await DbContext.Events.AddAsync(veaEvent);
        await DbContext.SaveChangesAsync();

        var command = Assert.IsType<Success<MakeEventPublicCommand>>(
            MakeEventPublicCommand.Create(veaEvent.Id.ToString())).Value;
        var handler = new MakeEventPublicHandler(EventRepository, UnitOfWork);

        // Act
        var result = await handler.HandleAsync(command);

        // Assert
        Assert.IsType<Success<None>>(result);
        await using var fresh = CreateFreshContext();
        var savedEvent = await fresh.Events.FindAsync(veaEvent.Id);
        Assert.Equal(EventVisibilityValue.Public, savedEvent!.Visibility.Value);
    }

    [Fact]
    public async Task HandleAsync_WhenEventNotFound_ReturnsFailure()
    {
        // Arrange
        var command = Assert.IsType<Success<MakeEventPublicCommand>>(
            MakeEventPublicCommand.Create(EventId.New().ToString())).Value;
        var handler = new MakeEventPublicHandler(EventRepository, UnitOfWork);

        // Act
        var result = await handler.HandleAsync(command);

        // Assert
        var failure = Assert.IsType<Failure<None>>(result);
        Assert.Contains(failure.Errors, e => e == EventErrors.VeaEvent.EventNotFound);
    }
}
