using AutoMapper;
using ElevatorSystem.API.Models.Entities;
using ElevatorSystem.API.Models.DTOs.Users;

namespace ElevatorSystem.API.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<User, UserDto>();
            CreateMap<RegisterDto, User>()
                .ForMember(dest => dest.PasswordHash, opt => opt.Ignore());
        }
    }
}
