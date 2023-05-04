using AutoMapper;
using Sektor.API.src.Dtos;
using Sektor.API.src.Entities;

namespace Sektor.API.src.Profiles;

public class MembershipTypeProfile : Profile
{
    public MembershipTypeProfile()
    {
        CreateMap<MembershipType, MembershipTypeDto>();
        CreateMap<MembershipCreationDto, MembershipType>();
    }
}