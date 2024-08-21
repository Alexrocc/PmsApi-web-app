using AutoMapper;
using PmsApi.DTOs;
using PmsApi.Models;

namespace PmsApi.Utilities;

class UserProfile : Profile
{
    public UserProfile()
    {
        // CreateMap<User, CreateUserDto>();
        CreateMap<CreateUserDto, User>();
        CreateMap<UpdateUserDto, User>();
        CreateMap<User, UserDto>();
    }
}