namespace Sektor.API.src.Entities;

public class MembershipType : Entity
{
    public string Name { get; set; }
    public decimal RegularPrice { get; set;}
    public decimal StudentPrice { get; set; }
    public ICollection<Membership> Memberships { get; set; } = new HashSet<Membership>();
}