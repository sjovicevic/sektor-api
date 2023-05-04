namespace Sektor.API.src.Dtos;

public class MembershipTypeDto
{
    public int Id { get; set; }
    public string Name { get; set; }
    public decimal RegularPrice { get; set; }
    public decimal StudentPrice { get; set; }
}