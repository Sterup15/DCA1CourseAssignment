using VEA.Infrastructure.EfcQueries.Models;

namespace VEA.Infrastructure.EfcQueries.SeedFactories;

public static class GuestSeedFactory
{
    public static List<Guest> CreateGuests() =>
    [
        new() { Id = "10000000-0000-0000-0000-000000000001", FirstName = "John",    LastName = "Doe",      ViaMail = "jdo@via.dk", ProfilePictureUrl = "https://robohash.org/1"  },
        new() { Id = "10000000-0000-0000-0000-000000000002", FirstName = "Jane",    LastName = "Smith",    ViaMail = "jsm@via.dk", ProfilePictureUrl = "https://robohash.org/2"  },
        new() { Id = "10000000-0000-0000-0000-000000000003", FirstName = "Alice",   LastName = "Johnson",  ViaMail = "ajo@via.dk", ProfilePictureUrl = "https://robohash.org/3"  },
        new() { Id = "10000000-0000-0000-0000-000000000004", FirstName = "Bob",     LastName = "Brown",    ViaMail = "bbo@via.dk", ProfilePictureUrl = "https://robohash.org/4"  },
        new() { Id = "10000000-0000-0000-0000-000000000005", FirstName = "Charlie", LastName = "Davis",    ViaMail = "cda@via.dk", ProfilePictureUrl = "https://robohash.org/5"  },
        new() { Id = "10000000-0000-0000-0000-000000000006", FirstName = "Eva",     LastName = "Wilson",   ViaMail = "ewi@via.dk", ProfilePictureUrl = "https://robohash.org/6"  },
        new() { Id = "10000000-0000-0000-0000-000000000007", FirstName = "Frank",   LastName = "Miller",   ViaMail = "fmi@via.dk", ProfilePictureUrl = "https://robohash.org/7"  },
        new() { Id = "10000000-0000-0000-0000-000000000008", FirstName = "Grace",   LastName = "Taylor",   ViaMail = "gta@via.dk", ProfilePictureUrl = "https://robohash.org/8"  },
        new() { Id = "10000000-0000-0000-0000-000000000009", FirstName = "Henry",   LastName = "Anderson", ViaMail = "han@via.dk", ProfilePictureUrl = "https://robohash.org/9"  },
        new() { Id = "10000000-0000-0000-0000-00000000000a", FirstName = "Iris",    LastName = "Thomas",   ViaMail = "ith@via.dk", ProfilePictureUrl = "https://robohash.org/10" },
    ];
}