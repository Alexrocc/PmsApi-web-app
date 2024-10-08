using System.Security.Claims;

namespace PmsApi.Utilities;

public class UserContextHelper : IUserContextHelper
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public UserContextHelper(IHttpContextAccessor context)
    {
        _httpContextAccessor = context;
    }

    public bool IsAdmin()
    {
        return _httpContextAccessor.HttpContext?.User.IsInRole("Admin") ?? false;
    }

    public string GetUserId()
    {
        return _httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier) ?? String.Empty;
    }
}