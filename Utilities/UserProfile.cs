using AutoMapper;
using PmsApi.DTOs;
using PmsApi.Models;
using Task = PmsApi.Models.Task;

namespace PmsApi.Utilities;

class UserProfile : Profile
{
    public UserProfile()
    {
        //user mappings
        CreateMap<CreateUserDto, User>();

        CreateMap<UpdateUserDto, User>();

        CreateMap<User, ManagerDto>();

        CreateMap<User, SimpleUserDto>();

        CreateMap<User, UserDto>().ForMember(d => d.Projects, opt => opt.MapFrom(src => src.Projects))
        .ForMember(d => d.Tasks, opt => opt.MapFrom(src => src.Tasks)); //mapping avoids recursivity of result

        //project mappings
        CreateMap<Project, ProjectDto>();

        CreateMap<ProjectDto, ProjectWithTaskDto>();

        CreateMap<CreateProjectDto, Project>();

        CreateMap<Project, ProjectWithTaskDto>()
        .ForMember(d => d.UsersManager, opt => opt.MapFrom(src => src.UsersManager));

        //tasks mappings
        CreateMap<Task, TaskDto>();

        CreateMap<ProjectCategory, CategoryDto>();

        CreateMap<Task, TaskAllDto>();

        CreateMap<CreateTaskDto, Task>();

        //task attachments mappings
        CreateMap<TaskAttachment, TaskAttachmentDto>();

        CreateMap<TaskAttachment, AttachmentWithTaskDto>()
        .ForMember(d => d.Task, opt => opt.MapFrom(src => src.Task));

        //Category mappings
        CreateMap<ProjectCategory, CategoryDto>();
        CreateMap<CreateCategoryDto, ProjectCategory>();

        //Status mappings
        CreateMap<Status, StatusDto>();
        CreateMap<CreateStatusDto, Status>();

        //Priority mappings
        CreateMap<Priority, PriorityDto>();
        CreateMap<CreatePriorityDto, Priority>();
    }
}