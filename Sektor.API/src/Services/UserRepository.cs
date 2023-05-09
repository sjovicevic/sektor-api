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

public class UserRepository : IUserRepository
{
    private readonly SektorContext _context;

    public UserRepository(SektorContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<User>> GetAllUsersAsync()
    {
        var users = await _context.Users
            .ToListAsync();
        return users;
    }

    public async Task<PagedList<User>> GetAllUsersAsync(UsersResourceParameters usersResourceParameters)
    {
        if (usersResourceParameters == null) 
        {
            throw new ArgumentNullException(nameof(usersResourceParameters));
        }
        var searchQuery = usersResourceParameters.SearchQuery;

        var collection = _context.Users as IQueryable<User>;

        if (!string.IsNullOrWhiteSpace(searchQuery))
        {
            searchQuery = searchQuery.Trim();
            collection = collection
            .Where(c => c.FirstName.Contains(searchQuery)
            || c.LastName.Contains(searchQuery)
            || c.Email.Contains(searchQuery))
            .OrderBy(c => c.FirstName);
        }
        
        return await PagedList<User>.CreateAsync(
            collection,
            usersResourceParameters.PageNumber,
            usersResourceParameters.PageSize);
        
    }

    public async Task<User?> GetUserByIdAsync(int id)
    {
        return await _context.Users.SingleOrDefaultAsync(u => u.Id == id);
    }

    public void AddNewUser(UserCreationDto userCreationDto)
    {
        var user = new User
        {
            FirstName = userCreationDto.FirstName,
            LastName = userCreationDto.LastName,
            Email = userCreationDto.Email,
            Student = userCreationDto.Student
        };

        _context.Users.Add(user);
    }

    public void UpdateUser(User user, UserCreationDto userCreationDto)
    {
        user.FirstName = userCreationDto.FirstName;
        user.LastName = userCreationDto.LastName;
        user.Email = userCreationDto.Email;
        user.Student = userCreationDto.Student;
        user.ModifiedAt = DateTime.Now;
    }

    public void DeleteUser(User user)
    {
        user.IsActive = false;
        user.IsDeleted = true;
        user.DeletedAt = DateTime.Now;
    }

    public async Task<bool> SaveChangesAsync()
    {
        return await _context.SaveChangesAsync() >= 0;
    }
}