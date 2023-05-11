using System.Collections;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Sektor.API.src.Contracts;
using Sektor.API.src.Dtos;
using Sektor.API.src.Entities;
using Sektor.API.src.Context;
using Sektor.API.src.ResourceParameters;
using Sektor.API.src.Helpers;

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

    public async Task<PagedList<Membership>> GetAllMembershipsAsync(MembershipsResourceParameters membershipsResourceParameters)
    {
        if(membershipsResourceParameters == null)
        {
            throw new ArgumentNullException(nameof(membershipsResourceParameters));
        }

        var membershipType = membershipsResourceParameters.MembershipType;
        var searchQuery = membershipsResourceParameters.SearchQuery;

        var collection = _context.Memberships.Include(u => u.User).Include(m => m.MembershipType).OrderBy(m => m.Id) as IQueryable<Membership>;
        var count = collection.Count();

        if(membershipType != null)
        {
            collection = collection
                .Where(a => a.MembershipType.Id == membershipType)
                .Include(u => u.User)
                .Include(m => m.MembershipType);
        }

        if(!string.IsNullOrWhiteSpace(searchQuery))
        {
            searchQuery = searchQuery.Trim();
            collection = collection
                .Where(a => a.User.FirstName.Contains(searchQuery) || a.User.LastName.Contains(searchQuery) || a.User.Email.Contains(searchQuery))
                .Include(u => u.User)
                .Include(m => m.MembershipType);
        }

        return await PagedList<Membership>.CreateAsync(
            collection,
            membershipsResourceParameters.PageNumber,
            membershipsResourceParameters.PageSize);

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
