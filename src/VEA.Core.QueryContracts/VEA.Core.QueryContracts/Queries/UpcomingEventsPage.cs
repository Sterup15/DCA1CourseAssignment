using VEA.Core.QueryContracts.Contract;

namespace VEA.Core.QueryContracts.Queries;

public record UpcomingEventsPageQuery(int PageNumber, int PageSize, string? TitleFilter)
    : IQuery<UpcomingEventsPageAnswer>;

public record UpcomingEventsPageAnswer(
    IEnumerable<UpcomingEventSummary> Events,
    int TotalPages);

public record UpcomingEventSummary(
    string Id,
    string Title,
    string Description,
    string Start,
    string End,
    int AttendeeCount,
    int GuestCapacity,
    bool IsPublic);