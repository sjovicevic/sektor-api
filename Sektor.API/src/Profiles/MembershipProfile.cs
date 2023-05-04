using AutoMapper;
using Sektor.API.src.Dtos;
using Sektor.API.src.Entities;

namespace Sektor.API.src.Profiles;

public class MembershipProfile : Profile
{
    public MembershipProfile()
    {
        CreateMap<Membership, MembershipDto>()
            .ForMember(dest => dest.UserFirstName, opt => opt.MapFrom(src => src.User.FirstName))
            .ForMember(dest => dest.UserLastName, opt => opt.MapFrom(src => src.User.LastName))
            .ForMember(dest => dest.UserEmail, opt => opt.MapFrom(src => src.User.Email))
            .ForMember(dest => dest.Student, opt => opt.MapFrom(src => src.User.Student))
            .ForMember(dest => dest.MembershipTypeName, opt => opt.MapFrom(src => src.MembershipType.Name))
            .ForMember(dest => dest.RegularPrice, opt => opt.MapFrom(src => src.MembershipType.RegularPrice))
            .ForMember(dest => dest.StudentPrice, opt => opt.MapFrom(src => src.MembershipType.StudentPrice));

        CreateMap<MembershipCreationDto, Membership>();
    }
}