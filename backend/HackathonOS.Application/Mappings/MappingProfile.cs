// HackathonOS.Application.Mappings/MappingProfile.cs
using AutoMapper;
using HackathonOS.Application.DTOs;
using HackathonOS.Domain.Entities;

namespace HackathonOS.Application.Mappings;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        // UserRequest -> User
        CreateMap<UserRequest, User>()
            .ForMember(dest => dest.PasswordHash, opt => opt.Ignore());

        // User -> UserResponse
        CreateMap<User, UserResponse>();
    }
}