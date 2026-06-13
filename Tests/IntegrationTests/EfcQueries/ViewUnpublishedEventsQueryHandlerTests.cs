using IntegrationTests.EfcQueries.Common;
using VEA.Infrastructure.EfcQueries;
using VEA.Core.QueryContracts.Queries;
using VEA.Infrastructure.EfcQueries.Queries;

namespace IntegrationTests.EfcQueries;

// Seed unpublished events:
//   Draft:     evt-07  Hackathon Prep
//   Ready:     evt-06  Workshop: Clean Architecture
//   Cancelled: evt-08  Semester Farewell
public class ViewUnpublishedEventsQueryHandlerTests
{
    [Fact]
    public async Task DraftsContainAllDraftEvents()
    {
        await using var ctx = ReadContextFactory.Create().Seed();
        var handler = new ViewUnpublishedEventsQueryHandler(ctx);

        var answer = await handler.HandleAsync(new ViewUnpublishedEventsQuery());

        Assert.Single(answer.Drafts);
        Assert.Equal("Hackathon Prep", answer.Drafts.Single().Title);
    }

    [Fact]
    public async Task ReadyContainsAllReadyEvents()
    {
        await using var ctx = ReadContextFactory.Create().Seed();
        var handler = new ViewUnpublishedEventsQueryHandler(ctx);

        var answer = await handler.HandleAsync(new ViewUnpublishedEventsQuery());

        Assert.Single(answer.Ready);
        Assert.Equal("Workshop: Clean Architecture", answer.Ready.Single().Title);
    }

    [Fact]
    public async Task CancelledContainsAllCancelledEvents()
    {
        await using var ctx = ReadContextFactory.Create().Seed();
        var handler = new ViewUnpublishedEventsQueryHandler(ctx);

        var answer = await handler.HandleAsync(new ViewUnpublishedEventsQuery());

        Assert.Single(answer.Cancelled);
        Assert.Equal("Semester Farewell", answer.Cancelled.Single().Title);
    }

    [Fact]
    public async Task ActiveEventsDoNotAppearInAnyGroup()
    {
        await using var ctx = ReadContextFactory.Create().Seed();
        var handler = new ViewUnpublishedEventsQueryHandler(ctx);

        var answer = await handler.HandleAsync(new ViewUnpublishedEventsQuery());

        var allTitles = answer.Drafts
            .Concat(answer.Ready)
            .Concat(answer.Cancelled)
            .Select(e => e.Title);

        Assert.DoesNotContain("Friday Bar", allTitles);
        Assert.DoesNotContain("Board Game Night", allTitles);
    }

    [Fact]
    public async Task EventTitleItemsIncludeId()
    {
        await using var ctx = ReadContextFactory.Create().Seed();
        var handler = new ViewUnpublishedEventsQueryHandler(ctx);

        var answer = await handler.HandleAsync(new ViewUnpublishedEventsQuery());

        Assert.All(
            answer.Drafts.Concat(answer.Ready).Concat(answer.Cancelled),
            item => Assert.False(string.IsNullOrEmpty(item.Id)));
    }
}