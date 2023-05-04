using Sektor.API.src.Entities;

namespace Sektor.API.src.Dtos;

public class UserDto 
{
    public int Id { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Email { get; set; }
    public bool Student { get; set; }
    public DateTime CreatedAt { get; set; }
    public bool IsActive { get; set; }
    public HashSet<Membership> Memberships { get; set; }
}