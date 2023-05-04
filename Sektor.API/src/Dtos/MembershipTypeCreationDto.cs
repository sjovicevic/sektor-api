namespace Sektor.API.src.Dtos;

public class MembershipTypeCreationDto
{
    public string Name { get; set; }
    public decimal RegularPrice { get; set; }
    public decimal StudentPrice { get; set; }
}