using Sektor.API.src.Entities;

namespace Sektor.API.src.Dtos;

public class MembershipDto
{
    public int Id { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    
    public string UserFirstName { get; set; }
    public string UserLastName { get; set; }
    public bool Student { get; set; }
    public string UserEmail { get; set; }

    public string MembershipTypeName { get; set; }
    public decimal RegularPrice { get; set; }
    public decimal StudentPrice { get; set; }
}