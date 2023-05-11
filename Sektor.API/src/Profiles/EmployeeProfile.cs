using AutoMapper;
using Sektor.API.src.Dtos;
using Sektor.API.src.Entities;

namespace Sektor.API.src.Profiles;

public class EmployeeProfile : Profile
{
    public EmployeeProfile()
    {
        CreateMap<Employee, EmployeeDto>();
    }
}