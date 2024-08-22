using AutoMapper;
using PmsApi.DTOs;
using PmsApi.Models;

namespace PmsApi.Utilities;

class UserProfile : Profile
{
    public UserProfile()
    {
        CreateMap<CreateUserDto, User>();

        CreateMap<UpdateUserDto, User>();

        CreateMap<User, ManagerDto>();

        CreateMap<User, UserDto>().ForMember(d => d.Projects, opt => opt.MapFrom(src => src.Projects))
        .ForMember(d => d.Tasks, opt => opt.MapFrom(src => src.Tasks)); //mapping avoids recursivity

        CreateMap<Project, ProjectDto>();

        CreateMap<Project, ProjectWithTaskDto>()
        .ForMember(d => d.Manager, opt => opt.MapFrom(src => src.UsersManager));

        CreateMap<Models.Task, TaskDto>();

        CreateMap<ProjectCategory, CategoryDto>();
    }
}