namespace VEA.Infrastructure.EfcQueries.Models;

public partial class Participation
{
    public string Id { get; set; } = null!;

    public string Status { get; set; } = null!;

    public string JoinReason { get; set; } = null!;

    public string GuestId { get; set; } = null!;

    public int Source { get; set; }

    public string EventId { get; set; } = null!;

    public virtual Event Event { get; set; } = null!;

    public virtual Guest Guest { get; set; } = null!;
}
