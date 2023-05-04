using Sektor.API.src.Dtos;
using Sektor.API.src.Entities;

namespace Sektor.API.src.Contracts;

public interface IMembershipTypeRepository
{
    Task<IEnumerable<MembershipType>> GetAllMembershipTypesAsync();
    Task<MembershipType?> GetMembershipTypeByIdAsync(int id);
    void AddNewMembershipType(MembershipTypeCreationDto dto);
    void DeleteMembershipType(MembershipType membershipType);
    void UpdateMembershipType(MembershipType membershipType, MembershipTypeCreationDto dto);
    Task<bool> SaveChangesAsync();
}