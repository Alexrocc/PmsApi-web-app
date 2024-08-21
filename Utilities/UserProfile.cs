using AutoMapper;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using PmsApi.DTOs;
using PmsApi.Models;

namespace PmsApi.Utilities;

class UserProfile : Profile
{
    public UserProfile()
    {
        CreateMap<CreateUserDto, User>();
        CreateMap<UpdateUserDto, User>();
        CreateMap<Project, ProjectDto>();
        CreateMap<Models.Task, TaskDto>();
        CreateMap<User, UserDto>().ForMember(d => d.Projects, opt => opt.MapFrom(src => src.Projects))
        .ForMember(d => d.Tasks, opt => opt.MapFrom(src => src.Tasks));
    }
}