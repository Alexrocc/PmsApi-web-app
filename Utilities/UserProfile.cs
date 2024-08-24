using AutoMapper;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using PmsApi.DTOs;
using PmsApi.Models;

namespace PmsApi.Utilities;

class UserProfile : Profile
{
    public UserProfile()
    {
        //user mappings
        CreateMap<CreateUserDto, User>();

        CreateMap<UpdateUserDto, User>();

        CreateMap<User, ManagerDto>();

        CreateMap<User, UserDto>().ForMember(d => d.Projects, opt => opt.MapFrom(src => src.Projects))
        .ForMember(d => d.Tasks, opt => opt.MapFrom(src => src.Tasks)); //mapping avoids recursivity of result

        //project mappings
        CreateMap<Project, ProjectDto>();

        CreateMap<ProjectDto, ProjectWithTaskDto>();

        CreateMap<CreateProjectDto, Project>();

        CreateMap<Project, ProjectWithTaskDto>()
        .ForMember(d => d.UsersManager, opt => opt.MapFrom(src => src.UsersManager));

        //tasks mappings
        CreateMap<Models.Task, TaskDto>();

        CreateMap<ProjectCategory, CategoryDto>();

        CreateMap<Models.Task, TaskAllDto>();

        //task attachments mappings
        CreateMap<TaskAttachment, TaskAttachmentDto>();
    }
}