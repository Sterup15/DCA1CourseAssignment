namespace VEA.Infrastructure.EfcQueries.Models;

public partial class Guest
{
    public string Id { get; set; } = null!;

    public string FirstName { get; set; } = null!;

    public string LastName { get; set; } = null!;

    public string ViaMail { get; set; } = null!;

    public string ProfilePictureUrl { get; set; } = null!;

    public virtual ICollection<Participation> Participations { get; set; } = new List<Participation>();
}
