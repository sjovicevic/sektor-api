namespace Sektor.API.src.Entities;

public class Membership : Entity
{
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public int UserId { get; set; }
    public User User { get; set; }
    public int MembershipTypeId { get; set; }
    public MembershipType MembershipType { get; set; }
}