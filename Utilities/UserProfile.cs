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
        CreateMap<User, UserDto>().ForMember(d => d.Projects, opt => opt.MapFrom(src => src.Projects))
        .ForMember(d => d.Tasks, opt => opt.MapFrom(src => src.Tasks)); //mapping avoids recursivity
        CreateMap<Project, ProjectDto>();
        CreateMap<Project, ProjectWithTaskDto>();
        CreateMap<Models.Task, TaskDto>();

    }
}