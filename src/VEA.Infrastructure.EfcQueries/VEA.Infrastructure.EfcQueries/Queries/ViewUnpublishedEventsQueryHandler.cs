using Microsoft.EntityFrameworkCore;
using VEA.Core.QueryContracts.Contract;
using VEA.Core.QueryContracts.Queries;

namespace VEA.Infrastructure.EfcQueries.Queries;

public class ViewUnpublishedEventsQueryHandler(VeadatabaseProductionContext ctx)
    : IQueryHandler<ViewUnpublishedEventsQuery, ViewUnpublishedEventsAnswer>
{
    public async Task<ViewUnpublishedEventsAnswer> HandleAsync(ViewUnpublishedEventsQuery query)
    {
        var drafts = await ctx.Events
            .Where(e => e.Status == "Draft")
            .Select(e => new EventTitleItem(e.Id, e.Title))
            .ToListAsync();

        var ready = await ctx.Events
            .Where(e => e.Status == "Ready")
            .Select(e => new EventTitleItem(e.Id, e.Title))
            .ToListAsync();

        var cancelled = await ctx.Events
            .Where(e => e.Status == "Cancelled")
            .Select(e => new EventTitleItem(e.Id, e.Title))
            .ToListAsync();

        return new ViewUnpublishedEventsAnswer(drafts, ready, cancelled);
    }
}