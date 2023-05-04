using Microsoft.AspNetCore.Mvc;
using Sektor.API.src.Dtos;
using Sektor.API.src.Entities;

namespace Sektor.API.src.Contracts;

public interface IUserRepository 
{
    Task<IEnumerable<User>> GetAllUsersAsync();
    Task<IEnumerable<User>> GetAllUsersAsync(string? name);
    Task<User?> GetUserByIdAsync(int id);
    void AddNewUser(UserCreationDto userCreationDto);
    void UpdateUser(User user, UserCreationDto userCreationDto);
    void DeleteUser(User user);
    Task<bool> SaveChangesAsync();
}