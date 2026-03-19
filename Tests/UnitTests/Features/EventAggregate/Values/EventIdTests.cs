using VEA.Core.Domain.Aggregates.VeaEventAggregate;
using VEA.Core.Tools.OperationResult;
using VEA.Core.Tools.OperationResult.Result;

namespace UnitTests.Features.EventAggregate.Values;

public sealed class EventIdTests
{
    [Fact]
    public void New_ShouldCreateNonEmptyGuid()
    {
        // Act
        var id = EventId.New();

        // Assert
        Assert.NotEqual(Guid.Empty, id.Value);
    }

    [Fact]
    public void New_Twice_ShouldCreateDifferentIds()
    {
        // Act
        var id1 = EventId.New();
        var id2 = EventId.New();

        // Assert
        Assert.NotEqual(id1, id2);
        Assert.NotEqual(id1.Value, id2.Value);
    }

    [Fact]
    public void From_ValidGuid_ShouldReturnSuccess_WithSameGuid()
    {
        // Arrange
        var guid = Guid.NewGuid();

        // Act
        Result<EventId> result = EventId.From(guid);

        // Assert
        var success = Assert.IsType<Success<EventId>>(result);
        Assert.Equal(guid, success.Value.Value);
    }

    [Fact]
    public void From_EmptyGuid_ShouldReturnFailure_WithExpectedError()
    {
        // Act
        Result<EventId> result = EventId.From(Guid.Empty);

        // Assert
        var failure = Assert.IsType<Failure<EventId>>(result);

        // If your ResultError type has Code:
        Assert.Contains(failure.Errors, e => e.Code == EventErrors.EventId.Empty.Code);
    }

    [Fact]
    public void From_EmptyGuid_ShouldContainSingleError()
    {
        // Act
        Result<EventId> result = EventId.From(Guid.Empty);

        // Assert
        var failure = Assert.IsType<Failure<EventId>>(result);
        Assert.Single(failure.Errors);
    }
}