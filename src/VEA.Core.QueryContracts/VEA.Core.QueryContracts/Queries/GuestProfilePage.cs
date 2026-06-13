using VEA.Core.QueryContracts.Contract;

namespace VEA.Core.QueryContracts.Queries;

public record GuestProfilePageQuery(string GuestId) : IQuery<GuestProfilePageAnswer>;

public record GuestProfilePageAnswer(
    string Id,
    string FullName,
    string Email,
    string ProfilePictureUrl,
    int PendingInvitationsCount,
    IEnumerable<UpcomingEventItem> UpcomingEvents,
    IEnumerable<PastEventItem> PastEvents);

public record UpcomingEventItem(string Id, string Title, int AttendeeCount, string Start);

public record PastEventItem(string Id, string Title);