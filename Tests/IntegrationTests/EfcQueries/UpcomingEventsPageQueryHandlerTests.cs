using IntegrationTests.EfcQueries.Common;
using VEA.Infrastructure.EfcQueries;
using VEA.Core.QueryContracts.Queries;
using VEA.Infrastructure.EfcQueries.Queries;

namespace IntegrationTests.EfcQueries;

// Seed data active events (fake time = 2024-03-25):
//   FUTURE active: evt-03 Board Game Night (2024-04-05, Public, 2 attendees)
//                  evt-05 Study Group Session (2024-04-12, Private, 1 attendee)
//   PAST  active:  evt-01 Friday Bar (2024-03-01), evt-02 Spring Festival (2024-03-15), evt-04 Networking (2024-03-20)
public class UpcomingEventsPageQueryHandlerTests
{
    private static readonly FakeSystemTime FakeTime = new(new DateTime(2024, 3, 25));

    [Fact]
    public async Task ReturnsOnlyActiveEventsAfterCurrentTime()
    {
        await using var ctx = ReadContextFactory.Create().Seed();
        var handler = new UpcomingEventsPageQueryHandler(ctx, FakeTime);

        var answer = await handler.HandleAsync(new UpcomingEventsPageQuery(1, 10, null));

        Assert.Equal(2, answer.Events.Count());
    }

    [Fact]
    public async Task EventsAreOrderedByDateAscending()
    {
        await using var ctx = ReadContextFactory.Create().Seed();
        var handler = new UpcomingEventsPageQueryHandler(ctx, FakeTime);

        var answer = await handler.HandleAsync(new UpcomingEventsPageQuery(1, 10, null));
        var events = answer.Events.ToList();

        Assert.Equal("Board Game Night", events[0].Title);
        Assert.Equal("Study Group Session", events[1].Title);
    }

    [Fact]
    public async Task PagingReturnsCorrectPage()
    {
        await using var ctx = ReadContextFactory.Create().Seed();
        var handler = new UpcomingEventsPageQueryHandler(ctx, FakeTime);

        var page1 = await handler.HandleAsync(new UpcomingEventsPageQuery(1, 1, null));
        var page2 = await handler.HandleAsync(new UpcomingEventsPageQuery(2, 1, null));

        Assert.Equal("Board Game Night", page1.Events.Single().Title);
        Assert.Equal("Study Group Session", page2.Events.Single().Title);
    }

    [Fact]
    public async Task TotalPagesCalculatedCorrectly()
    {
        await using var ctx = ReadContextFactory.Create().Seed();
        var handler = new UpcomingEventsPageQueryHandler(ctx, FakeTime);

        var answer = await handler.HandleAsync(new UpcomingEventsPageQuery(1, 1, null));

        Assert.Equal(2, answer.TotalPages);
    }

    [Fact]
    public async Task TitleFilterNarrowsResults()
    {
        await using var ctx = ReadContextFactory.Create().Seed();
        var handler = new UpcomingEventsPageQueryHandler(ctx, FakeTime);

        var answer = await handler.HandleAsync(new UpcomingEventsPageQuery(1, 10, "Board"));

        Assert.Single(answer.Events);
        Assert.Equal("Board Game Night", answer.Events.Single().Title);
    }

    [Fact]
    public async Task NullTitleFilterReturnsAllUpcomingEvents()
    {
        await using var ctx = ReadContextFactory.Create().Seed();
        var handler = new UpcomingEventsPageQueryHandler(ctx, FakeTime);

        var answer = await handler.HandleAsync(new UpcomingEventsPageQuery(1, 10, null));

        Assert.Equal(2, answer.Events.Count());
    }

    [Fact]
    public async Task EventSummaryIncludesAttendeeCountAndCapacity()
    {
        await using var ctx = ReadContextFactory.Create().Seed();
        var handler = new UpcomingEventsPageQueryHandler(ctx, FakeTime);

        var answer = await handler.HandleAsync(new UpcomingEventsPageQuery(1, 10, null));
        var boardGameNight = answer.Events.Single(e => e.Title == "Board Game Night");

        Assert.Equal(2, boardGameNight.AttendeeCount);
        Assert.Equal(20, boardGameNight.GuestCapacity);
        Assert.True(boardGameNight.IsPublic);
    }

    [Fact]
    public async Task PrivateEventHasIsPublicFalse()
    {
        await using var ctx = ReadContextFactory.Create().Seed();
        var handler = new UpcomingEventsPageQueryHandler(ctx, FakeTime);

        var answer = await handler.HandleAsync(new UpcomingEventsPageQuery(1, 10, null));
        var studyGroup = answer.Events.Single(e => e.Title == "Study Group Session");

        Assert.False(studyGroup.IsPublic);
    }
}