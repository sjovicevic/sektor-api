using System.Collections;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Sektor.API.src.Contracts;
using Sektor.API.src.Dtos;
using Sektor.API.src.Entities;
using Sektor.API.src.Context;

namespace Sektor.API.src.Services;

public class MembershipRepository : IMembershipRepository
{
    private readonly SektorContext _context;

    public MembershipRepository(SektorContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Membership>> GetAllMembershipsAsync()
    {
        return await _context
            .Memberships
            .Include(u => u.User)
            .Include(m => m.MembershipType)
            .ToListAsync();
    }

    public async Task<Membership?> GetMembershipByIdAsync(int id)
    {
        var membership = await _context
            .Memberships
            .Include(u => u.User)
            .Include(m => m.MembershipType)
            .SingleOrDefaultAsync(m => m.Id == id);

        return membership;
    }

    public void AddNewMembership(
        MembershipCreationDto membershipCreationDto,
        User user,
        MembershipType membershipType)
    {

        var membership = new Membership
        {
            StartDate = membershipCreationDto.StartDate,
            EndDate = membershipCreationDto.EndDate,
            User = user,
            MembershipType = membershipType
        };

        _context.Memberships.Add(membership);
    }

    public void UpdateMembership(
        Membership membership,
        MembershipCreationDto dto)
    {
        membership.StartDate = dto.StartDate;
        membership.EndDate = dto.EndDate;
    }


    public void DeleteMembership(Membership membership)
    {
        membership.IsDeleted = true;
        membership.DeletedAt = DateTime.Now;
        membership.IsActive = false;
    }

    public async Task<bool> SaveChangesAsync()
    {
        return await _context.SaveChangesAsync() >= 0;
    }
}
