using Microsoft.AspNetCore.Mvc;
using Sektor.API.src.Dtos;
using Sektor.API.src.Entities;
using Sektor.API.src.ResourceParameters;
using Sektor.API.src.Helpers;

namespace Sektor.API.src.Contracts;

public interface IUserRepository 
{
    IEnumerable<User> GetAllUsers();
    Task<IEnumerable<User>> GetAllUsersAsync();
    Task<PagedList<User>> GetAllUsersAsync(UsersResourceParameters usersResourceParameters);
    Task<User?> GetUserByIdAsync(int id);
    void AddNewUser(UserCreationDto userCreationDto);
    void UpdateUser(User user, UserCreationDto userCreationDto);
    void DeleteUser(User user);
    Task<bool> SaveChangesAsync();
}