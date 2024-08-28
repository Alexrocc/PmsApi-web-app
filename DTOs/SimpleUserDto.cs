namespace PmsApi.DTOs;

public record SimpleUserDto
(
    string Id,

    string UserName,

    string FirstName,

    string Lastname,

    string Email,

    string Password
);