using AutoMapper;
using NexusFit.Auth.API.Dtos;
using NexusFit.Auth.API.Entities;

namespace NexusFit.Auth.API;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<RegisterDto, ApplicationUser>()
            .ForMember(q => q.UserName, opt => opt.MapFrom(s => s.Email));
    }
}
