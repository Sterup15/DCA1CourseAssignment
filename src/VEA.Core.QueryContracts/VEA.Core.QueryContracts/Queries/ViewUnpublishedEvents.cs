using VEA.Core.QueryContracts.Contract;

namespace VEA.Core.QueryContracts.Queries;

public record ViewUnpublishedEventsQuery() : IQuery<ViewUnpublishedEventsAnswer>;

public record ViewUnpublishedEventsAnswer(
    IEnumerable<EventTitleItem> Drafts,
    IEnumerable<EventTitleItem> Ready,
    IEnumerable<EventTitleItem> Cancelled);

public record EventTitleItem(string Id, string Title);