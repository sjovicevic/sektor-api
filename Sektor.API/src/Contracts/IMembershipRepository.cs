using Sektor.API.src.Dtos;
using Sektor.API.src.Entities;

namespace Sektor.API.src.Contracts;

public interface IMembershipRepository
{
    Task<IEnumerable<Membership>> GetAllMembershipsAsync();
    Task<Membership?> GetMembershipByIdAsync(int id);
    void AddNewMembership(MembershipCreationDto membershipCreationDto, User user, MembershipType membershipType);
    void UpdateMembership(Membership membership, MembershipCreationDto dto);
    void DeleteMembership(Membership membership);
    Task<bool> SaveChangesAsync();
}