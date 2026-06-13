using VEA.Infrastructure.EfcQueries.Models;

namespace VEA.Infrastructure.EfcQueries.SeedFactories;

// Visibility: 0 = Private, 1 = Public
// Status: "Draft", "Ready", "Active", "Cancelled"
// Events span 2024-03-01 to 2024-04-30 — set fake system time in between when testing
public static class EventSeedFactory
{
    public static List<Event> CreateEvents() =>
    [
        new()
        {
            Id            = "20000000-0000-0000-0000-000000000001",
            Title         = "Friday Bar",
            Description   = "Come for cheap beer and great company.",
            Status        = "Active",
            Visibility    = 1,
            TimeRangeStart = "2024-03-01T18:00:00",
            TimeRangeEnd   = "2024-03-01T23:00:00",
            GuestCapacity = 50,
            Location      = null
        },
        new()
        {
            Id            = "20000000-0000-0000-0000-000000000002",
            Title         = "VIA Spring Festival",
            Description   = "Annual spring festival with music and food.",
            Status        = "Active",
            Visibility    = 1,
            TimeRangeStart = "2024-03-15T12:00:00",
            TimeRangeEnd   = "2024-03-15T20:00:00",
            GuestCapacity = 100,
            Location      = null
        },
        new()
        {
            Id            = "20000000-0000-0000-0000-000000000003",
            Title         = "Board Game Night",
            Description   = "A cozy evening of board games and snacks.",
            Status        = "Active",
            Visibility    = 1,
            TimeRangeStart = "2024-04-05T17:00:00",
            TimeRangeEnd   = "2024-04-05T22:00:00",
            GuestCapacity = 20,
            Location      = null
        },
        new()
        {
            Id            = "20000000-0000-0000-0000-000000000004",
            Title         = "Private Networking Event",
            Description   = "Exclusive networking session for selected students.",
            Status        = "Active",
            Visibility    = 0,
            TimeRangeStart = "2024-03-20T18:00:00",
            TimeRangeEnd   = "2024-03-20T21:00:00",
            GuestCapacity = 15,
            Location      = null
        },
        new()
        {
            Id            = "20000000-0000-0000-0000-000000000005",
            Title         = "Study Group Session",
            Description   = "Focused study session for exam preparation.",
            Status        = "Active",
            Visibility    = 0,
            TimeRangeStart = "2024-04-12T14:00:00",
            TimeRangeEnd   = "2024-04-12T17:00:00",
            GuestCapacity = 10,
            Location      = null
        },
        new()
        {
            Id            = "20000000-0000-0000-0000-000000000006",
            Title         = "Workshop: Clean Architecture",
            Description   = "Hands-on workshop covering clean architecture principles.",
            Status        = "Ready",
            Visibility    = 1,
            TimeRangeStart = "2024-04-25T09:00:00",
            TimeRangeEnd   = "2024-04-25T16:00:00",
            GuestCapacity = 30,
            Location      = null
        },
        new()
        {
            Id            = "20000000-0000-0000-0000-000000000007",
            Title         = "Hackathon Prep",
            Description   = "Team formation and planning session for the upcoming hackathon.",
            Status        = "Draft",
            Visibility    = 1,
            TimeRangeStart = "2024-04-30T10:00:00",
            TimeRangeEnd   = "2024-04-30T18:00:00",
            GuestCapacity = 25,
            Location      = null
        },
        new()
        {
            Id            = "20000000-0000-0000-0000-000000000008",
            Title         = "Semester Farewell",
            Description   = "Farewell party for graduating students.",
            Status        = "Cancelled",
            Visibility    = 1,
            TimeRangeStart = "2024-03-28T18:00:00",
            TimeRangeEnd   = "2024-03-28T23:00:00",
            GuestCapacity = 50,
            Location      = null
        },
    ];
}
