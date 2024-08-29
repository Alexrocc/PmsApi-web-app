namespace PmsApi.Utilities;

public interface IUserContextHelper
{
    bool IsAdmin();
    string GetUserId();
}