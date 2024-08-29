namespace PmsApi.DTOs;

public record ManagerDto
(
    string UserId,

    string UserName,

    string FirstName,

    string Lastname,

    string Email,

    string Password,

    int RoleId
);