using VEA.Core.QueryContracts.Contract;

namespace VEA.Core.QueryContracts.Queries;

public record ViewSingleEventQuery(string EventId, int GuestOffset, int GuestPageSize)
    : IQuery<ViewSingleEventAnswer>;

public record ViewSingleEventAnswer(
    string Id,
    string Title,
    string Description,
    string? Location,
    string Start,
    string End,
    bool IsPublic,
    int AttendeeCount,
    int GuestCapacity,
    IEnumerable<AttendeeItem> Attendees);

public record AttendeeItem(string Id, string FullName, string ProfilePictureUrl);