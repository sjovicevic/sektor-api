using Microsoft.EntityFrameworkCore;
using Sektor.API.src.Contracts;
using Sektor.API.src.Dtos;
using Sektor.API.src.Entities;
using Sektor.API.src.Context;

namespace Sektor.API.src.Services;

public class MembershipTypeRepository : IMembershipTypeRepository
{
    private readonly SektorContext _context;

    public MembershipTypeRepository(SektorContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<MembershipType>> GetAllMembershipTypesAsync()
    {
        return await _context.MembershipTypes.ToListAsync();
    }

    public async Task<MembershipType?> GetMembershipTypeByIdAsync(int id)
    {
        return await _context.MembershipTypes.SingleOrDefaultAsync(m => m.Id == id);
    }

    public void AddNewMembershipType (MembershipTypeCreationDto dto)
    {
        var membershipType = new MembershipType
        {
            Name = dto.Name,
            RegularPrice = dto.RegularPrice,
            StudentPrice = dto.StudentPrice
        };

        _context.MembershipTypes.Add(membershipType);
    }
    
    public void UpdateMembershipType(MembershipType membershipType, MembershipTypeCreationDto dto)
    {
        membershipType.Name = dto.Name;
        membershipType.StudentPrice = dto.StudentPrice;
        membershipType.RegularPrice = dto.RegularPrice;
    }

    public void DeleteMembershipType(MembershipType membershipType)
    {
        membershipType.IsDeleted = true;
        membershipType.DeletedAt = DateTime.Now;
        membershipType.IsActive = false;
    }

    public async Task<bool> SaveChangesAsync()
    {
        return await _context.SaveChangesAsync() >= 0;
    }

    public bool CheckIfMembershipExists(MembershipType membershipType)
    {
        var memberships = _context.Memberships.Where(m => m.MembershipType.Id == membershipType.Id);
        
        if(memberships != null)
        {
            return true;
        }

        return false;
    }
}