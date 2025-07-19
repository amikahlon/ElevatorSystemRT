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
            // User
            CreateMap<User, UserDto>();
            CreateMap<RegisterDto, User>()
                .ForMember(dest => dest.PasswordHash, opt => opt.Ignore()); //

            // Building
            CreateMap<Building, BuildingDto>();
            CreateMap<CreateBuildingDto, Building>();

            // Elevator
            CreateMap<CreateElevatorDto, Elevator>();
            CreateMap<Elevator, ElevatorDto>();
            CreateMap<ElevatorDto, Elevator>();

            // Elevator Call
            CreateMap<ElevatorCall, ElevatorCallDto>();
            CreateMap<CreateElevatorCallDto, ElevatorCall>(); // Ensure this mapping handles `Direction` enum too.
                                                                           // AutoMapper usually handles direct enum to enum mapping if names match.

            // Elevator Call Assignment
            CreateMap<ElevatorCallAssignment, ElevatorCallAssignmentDto>();
            CreateMap<CreateElevatorCallAssignmentDto, ElevatorCallAssignment>();
        }
    }
}