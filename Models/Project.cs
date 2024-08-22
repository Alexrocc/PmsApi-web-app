﻿namespace PmsApi.Models;

public partial class Project
{
    public int ProjectId { get; set; }
    public string ProjectName { get; set; } = null!;
    public string? Description { get; set; }
    public int StatusId { get; set; }
    public virtual Status? Status { get; set; }
    public int PriorityId { get; set; }
    public virtual Priority? Priority { get; set; }
    public DateOnly? StartDate { get; set; }
    public DateOnly? EndDate { get; set; }
    public int ProjectCategoriesId { get; set; }
    public int UsersManagerId { get; set; }
    public ProjectCategory? ProjectCategory { get; set; }
    public ICollection<Models.Task>? Tasks { get; set; }
    public User? UsersManager { get; set; }
}