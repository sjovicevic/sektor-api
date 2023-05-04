namespace Sektor.API.src.Entities;

public class User : Entity
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string? Email { get; set; }
    public bool Student { get; set; }

    public ICollection<Membership> Memberships { get; set; } = new HashSet<Membership>();
}