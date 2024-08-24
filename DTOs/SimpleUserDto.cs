namespace PmsApi.DTOs;

public record SimpleUserDto
(
    int UserId,

    string UserName,

    string FirstName,

    string Lastname,

    string Email,

    string Password,

    int RoleId
);