using AutoMapper;
using Sektor.API.src.Dtos;
using Sektor.API.src.Entities;

namespace Sektor.API.src.Profiles;

public class UserProfile : Profile
{
    public UserProfile()
    {
        CreateMap<User, UserDto>();
        CreateMap<UserCreationDto, User>();
    }
}