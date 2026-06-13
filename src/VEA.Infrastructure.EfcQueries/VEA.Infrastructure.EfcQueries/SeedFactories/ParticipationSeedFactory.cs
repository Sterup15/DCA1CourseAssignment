using VEA.Infrastructure.EfcQueries.Models;

namespace VEA.Infrastructure.EfcQueries.SeedFactories;

// Status: "Attending", "Invited", "Rejected"
// Source: 0 = Public (guest joined directly), 1 = Private (guest was invited)
public static class ParticipationSeedFactory
{
    public static void Seed(VeadatabaseProductionContext context)
    {
        var participations = new List<Participation>
        {
            // Event 1 (Friday Bar — Active, Public): 3 attendees
            new() { Id = "30000000-0000-0000-0000-000000000001", EventId = "20000000-0000-0000-0000-000000000001", GuestId = "10000000-0000-0000-0000-000000000001", Status = "Attending", Source = 0, JoinReason = "" },
            new() { Id = "30000000-0000-0000-0000-000000000002", EventId = "20000000-0000-0000-0000-000000000001", GuestId = "10000000-0000-0000-0000-000000000002", Status = "Attending", Source = 0, JoinReason = "" },
            new() { Id = "30000000-0000-0000-0000-000000000003", EventId = "20000000-0000-0000-0000-000000000001", GuestId = "10000000-0000-0000-0000-000000000003", Status = "Attending", Source = 0, JoinReason = "" },

            // Event 2 (VIA Spring Festival — Active, Public): 3 attendees
            new() { Id = "30000000-0000-0000-0000-000000000004", EventId = "20000000-0000-0000-0000-000000000002", GuestId = "10000000-0000-0000-0000-000000000004", Status = "Attending", Source = 0, JoinReason = "" },
            new() { Id = "30000000-0000-0000-0000-000000000005", EventId = "20000000-0000-0000-0000-000000000002", GuestId = "10000000-0000-0000-0000-000000000005", Status = "Attending", Source = 0, JoinReason = "" },
            new() { Id = "30000000-0000-0000-0000-000000000006", EventId = "20000000-0000-0000-0000-000000000002", GuestId = "10000000-0000-0000-0000-000000000006", Status = "Attending", Source = 0, JoinReason = "" },

            // Event 3 (Board Game Night — Active, Public): 2 attendees
            new() { Id = "30000000-0000-0000-0000-000000000007", EventId = "20000000-0000-0000-0000-000000000003", GuestId = "10000000-0000-0000-0000-000000000007", Status = "Attending", Source = 0, JoinReason = "" },
            new() { Id = "30000000-0000-0000-0000-000000000008", EventId = "20000000-0000-0000-0000-000000000003", GuestId = "10000000-0000-0000-0000-000000000008", Status = "Attending", Source = 0, JoinReason = "" },

            // Event 4 (Private Networking — Active, Private): 1 attending, 1 invited, 1 rejected
            new() { Id = "30000000-0000-0000-0000-000000000009", EventId = "20000000-0000-0000-0000-000000000004", GuestId = "10000000-0000-0000-0000-000000000001", Status = "Attending", Source = 1, JoinReason = "I am interested in networking." },
            new() { Id = "30000000-0000-0000-0000-00000000000a", EventId = "20000000-0000-0000-0000-000000000004", GuestId = "10000000-0000-0000-0000-000000000003", Status = "Invited",   Source = 1, JoinReason = "" },
            new() { Id = "30000000-0000-0000-0000-00000000000b", EventId = "20000000-0000-0000-0000-000000000004", GuestId = "10000000-0000-0000-0000-000000000005", Status = "Rejected",  Source = 1, JoinReason = "" },

            // Event 5 (Study Group — Active, Private): 1 attending, 1 invited
            new() { Id = "30000000-0000-0000-0000-00000000000c", EventId = "20000000-0000-0000-0000-000000000005", GuestId = "10000000-0000-0000-0000-000000000002", Status = "Attending", Source = 1, JoinReason = "I need help preparing for the exam." },
            new() { Id = "30000000-0000-0000-0000-00000000000d", EventId = "20000000-0000-0000-0000-000000000005", GuestId = "10000000-0000-0000-0000-000000000004", Status = "Invited",   Source = 1, JoinReason = "" },
        };

        context.Participations.AddRange(participations);
    }
}
