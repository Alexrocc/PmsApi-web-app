using Microsoft.EntityFrameworkCore;
using PmsApi.Models;
using Task = PmsApi.Models.Task;

namespace PmsApi.Utilities;

public static class QueryHelper
{
    public static IQueryable<User> ApplyUserIncludes(IQueryable<User> query, string include = "")
    {
        var includes = include.Split(",", StringSplitOptions.RemoveEmptyEntries);
        foreach (var item in includes)
        {
            switch (item)
            {
                case "tasks":
                    query.Include(x => x.Tasks);
                    break;

                case "projects":
                    query.Include(x => x.Projects);
                    break;
            }
        }
        return query;
    }
    public static IQueryable<Task> ApplyTaskIncludes(IQueryable<Task> query, string include = "")
    {
        var includes = include.Split(",", StringSplitOptions.RemoveEmptyEntries);
        foreach (var item in includes)
        {
            switch (item)
            {
                case "user":
                    query.Include(x => x.AssignedUser);
                    break;

                case "project":
                    query.Include(x => x.Project);
                    break;

                case "attachments":
                    query.Include(x => x.TaskAttachments);
                    break;
            }
        }
        return query;
    }
    public static IQueryable<Project> ApplyProjectIncludes(IQueryable<Project> query, string include = "")
    {
        var includes = include.Split(",", StringSplitOptions.RemoveEmptyEntries);
        foreach (var item in includes)
        {
            switch (item)
            {
                case "tasks":
                    query.Include(x => x.Tasks);
                    break;

                case "manager":
                    query.Include(x => x.UsersManager);
                    break;

                case "category":
                    query.Include(x => x.ProjectCategory);
                    break;
            }
        }
        return query;
    }
}