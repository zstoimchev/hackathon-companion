// HackathonOS.Application.Mappings/MappingProfile.cs
using AutoMapper;
using HackathonOS.Application.DTOs;
using HackathonOS.Domain.Entities;

namespace HackathonOS.Application.Mappings;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        // UserRequest → User
        CreateMap<UserRequest, User>()
            .ForMember(dest => dest.PasswordHash, opt => opt.Ignore()) // handled by AuthService
            .ForMember(dest => dest.Role, opt => opt.MapFrom(src => src.Role));

        // User → UserResponse
        CreateMap<User, UserResponse>()
            .ForMember(dest => dest.Role, opt => opt.MapFrom(src => src.Role.ToString()));
    }
}