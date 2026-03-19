using UnitTests.Common;
using VEA.Core.Application.AppEntry.Commands.VeaEventCommands;
using VEA.Core.Application.Features.VeaEventHandlers.ReadyEvent;
using VEA.Core.Domain.Aggregates.VeaEventAggregate;
using VEA.Core.Tools.OperationResult;
using VEA.Core.Tools.OperationResult.Result;

namespace UnitTests.Features.EventAggregate.ReadyEvent;

public class ReadyEventHandlerTests
{
    [Fact]
    public async Task HandleAsync_WhenEventIsDraftWithValidState_ReturnsSuccess()
    {
        // Arrange
        var veaEvent = VeaEventTestFactory.CreateEvent();
        var now = VeaEventTestFactory.DefaultNow;
        veaEvent.UpdateTitle(VeaEventTestFactory.CreateTitle("Valid Title"));
        veaEvent.UpdateDescription(VeaEventTestFactory.CreateDescription("Valid description"));
        veaEvent.UpdateTimeRange(
            VeaEventTestFactory.CreateTimeRange(
                new DateTime(2030, 8, 25, 13, 0, 0, DateTimeKind.Utc),
                new DateTime(2030, 8, 25, 15, 0, 0, DateTimeKind.Utc)),
            now);

        var command = Assert.IsType<Success<ReadyEventCommand>>(
            ReadyEventCommand.Create(veaEvent.Id.ToString())).Value;
        var handler = new ReadyEventHandler(new FakeVeaEventRepository(veaEvent), new FakeUnitOfWork());

        // Act
        var result = await handler.HandleAsync(command);

        // Assert
        Assert.IsType<Success<None>>(result);
    }

    [Fact]
    public async Task HandleAsync_WhenEventNotFound_ReturnsFailure()
    {
        // Arrange
        var command = Assert.IsType<Success<ReadyEventCommand>>(
            ReadyEventCommand.Create(EventId.New().ToString())).Value;
        var handler = new ReadyEventHandler(new FakeVeaEventRepository(null), new FakeUnitOfWork());

        // Act
        var result = await handler.HandleAsync(command);

        // Assert
        var failure = Assert.IsType<Failure<None>>(result);
        Assert.Contains(failure.Errors, e => e == EventErrors.VeaEvent.EventNotFound);
    }
}
