namespace VEA.Infrastructure.EfcQueries.Models;

public partial class Event
{
    public string Id { get; set; } = null!;

    public string Title { get; set; } = null!;

    public string Description { get; set; } = null!;

    public string? TimeRangeStart { get; set; }

    public string? TimeRangeEnd { get; set; }

    public int Visibility { get; set; }

    public string Status { get; set; } = null!;

    public string? Location { get; set; }

    public int GuestCapacity { get; set; }

    public virtual ICollection<Participation> Participations { get; set; } = new List<Participation>();
}
