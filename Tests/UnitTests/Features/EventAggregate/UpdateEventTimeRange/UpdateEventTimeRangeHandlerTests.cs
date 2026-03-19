using UnitTests.Common;
using VEA.Core.Application.AppEntry.Commands.VeaEventCommands;
using VEA.Core.Application.Features.VeaEventHandlers.UpdateEventTimeRange;
using VEA.Core.Domain.Aggregates.VeaEventAggregate;
using VEA.Core.Tools.OperationResult;
using VEA.Core.Tools.OperationResult.Result;

namespace UnitTests.Features.EventAggregate.UpdateEventTimeRange;

public class UpdateEventTimeRangeHandlerTests
{
    [Fact]
    public async Task HandleAsync_WhenEventExists_ReturnsSuccess()
    {
        // Arrange
        var veaEvent = VeaEventTestFactory.CreateEvent();
        var start = new DateTime(2030, 8, 25, 13, 0, 0, DateTimeKind.Utc);
        var end = new DateTime(2030, 8, 25, 15, 0, 0, DateTimeKind.Utc);
        var command = Assert.IsType<Success<UpdateEventTimeRangeCommand>>(
            UpdateEventTimeRangeCommand.Create(veaEvent.Id.ToString(), start, end)).Value;
        var handler = new UpdateEventTimeRangeHandler(new FakeVeaEventRepository(veaEvent), new FakeUnitOfWork());

        // Act
        var result = await handler.HandleAsync(command);

        // Assert
        Assert.IsType<Success<None>>(result);
    }

    [Fact]
    public async Task HandleAsync_WhenEventNotFound_ReturnsFailure()
    {
        // Arrange
        var start = new DateTime(2030, 8, 25, 13, 0, 0, DateTimeKind.Utc);
        var end = new DateTime(2030, 8, 25, 15, 0, 0, DateTimeKind.Utc);
        var command = Assert.IsType<Success<UpdateEventTimeRangeCommand>>(
            UpdateEventTimeRangeCommand.Create(EventId.New().ToString(), start, end)).Value;
        var handler = new UpdateEventTimeRangeHandler(new FakeVeaEventRepository(null), new FakeUnitOfWork());

        // Act
        var result = await handler.HandleAsync(command);

        // Assert
        var failure = Assert.IsType<Failure<None>>(result);
        Assert.Contains(failure.Errors, e => e == EventErrors.VeaEvent.EventNotFound);
    }
}
