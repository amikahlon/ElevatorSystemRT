using AutoMapper;
using ElevatorSystem.API.Models.Entities;
using ElevatorSystem.API.Models.DTOs.Users;
using ElevatorSystem.API.Models.DTOs.Buildings;
using ElevatorSystem.API.Models.DTOs.Elevators;

namespace ElevatorSystem.API.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<User, UserDto>();
            CreateMap<RegisterDto, User>()
                .ForMember(dest => dest.PasswordHash, opt => opt.Ignore());

            CreateMap<Building, BuildingDto>();
            CreateMap<CreateBuildingDto, Building>();

            CreateMap<CreateElevatorDto, Elevator>();
            CreateMap<Elevator, ElevatorDto>();

            CreateMap<ElevatorCall, ElevatorCallDto>();
            CreateMap<CreateElevatorCallDto, ElevatorCall>();

            CreateMap<ElevatorCallAssignment, ElevatorCallAssignmentDto>();
            CreateMap<CreateElevatorCallAssignmentDto, ElevatorCallAssignment>();


    
        }
    }
}
