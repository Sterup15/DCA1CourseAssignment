using VEA.Core.Application.AppEntry.Commands.VeaEventCommands;
using VEA.Core.Domain.Aggregates.VeaEventAggregate;
using VEA.Core.Tools.OperationResult;
using VEA.Core.Tools.OperationResult.Result;

namespace UnitTests.Features.EventAggregate.UpdateEventTimeRange;

public class UpdateEventTimeRangeCommandTests
{
    [Fact]
    public void Create_WithValidInputs_ReturnsSuccess()
    {
        // Arrange
        var eventId = EventId.New().ToString();
        var start = new DateTime(2030, 8, 25, 13, 0, 0, DateTimeKind.Utc);
        var end = new DateTime(2030, 8, 25, 15, 0, 0, DateTimeKind.Utc);

        // Act
        var result = UpdateEventTimeRangeCommand.Create(eventId, start, end);

        // Assert
        Assert.IsType<Success<UpdateEventTimeRangeCommand>>(result);
    }

    [Fact]
    public void Create_WithDefaultStartTime_ReturnsFailure()
    {
        // Arrange
        var eventId = EventId.New().ToString();

        // Act
        var result = UpdateEventTimeRangeCommand.Create(eventId, default, default);

        // Assert
        var failure = Assert.IsType<Failure<UpdateEventTimeRangeCommand>>(result);
        Assert.Contains(failure.Errors, e => e == EventErrors.EventTimeRange.StartEmpty);
    }
}
