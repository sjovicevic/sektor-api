using System.Collections;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Sektor.API.src.Contracts;
using Sektor.API.src.Dtos;
using Sektor.API.src.Entities;
using Sektor.API.src.Context;

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

    public async Task<IEnumerable<User>> GetAllUsersAsync(string? name)
    {
        if (string.IsNullOrEmpty(name))
        {
            return await GetAllUsersAsync();
        }

        name = name.Trim();

        return await _context.Users
            .Where(c => c.FirstName == name)
            .OrderBy(c => c.FirstName)
            .ToListAsync();
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