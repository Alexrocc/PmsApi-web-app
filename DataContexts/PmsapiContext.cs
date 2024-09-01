using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using PmsApi.Models;

namespace PmsApi.DataContexts;

public class PmsapiContext : IdentityDbContext
{
    private readonly string connString = String.Empty;
    public PmsapiContext()
    {
    }
    public PmsapiContext(string connString)
    {
        this.connString = connString;
    }

    public PmsapiContext(DbContextOptions<PmsapiContext> options)
        : base(options)
    {
    }

    public DbSet<Project> Projects { get; set; }

    public DbSet<ProjectCategory> ProjectCategories { get; set; }

    public new DbSet<Role> Roles { get; set; }

    public DbSet<Models.Task> Tasks { get; set; }

    public DbSet<TaskAttachment> TaskAttachments { get; set; }

    public new DbSet<User> Users { get; set; }

    public DbSet<Status> Statuses { get; set; }

    public DbSet<Priority> Priorities { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (connString != String.Empty)
        {
            optionsBuilder.UseMySql(connString, ServerVersion.AutoDetect(connString));
        }
    }


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Project>(entity =>
        {
            entity.HasKey(e => e.ProjectId).HasName("PRIMARY");

            entity.ToTable("projects");

            entity.HasIndex(e => e.ProjectCategoriesId, "project_categories_id");

            entity.HasIndex(e => e.UsersManagerId, "users_manager_id");

            entity.HasIndex(e => e.StatusId, "status_id");

            entity.HasIndex(e => e.PriorityId, "priority_id");
            entity.HasIndex(e => e.ProjectName).IsUnique();

            entity.Property(e => e.ProjectId)
                .HasColumnType("int(11)")
                .HasColumnName("id");
            entity.Property(e => e.Description)
                .HasMaxLength(255)
                .HasColumnName("description");
            entity.Property(e => e.EndDate)
                .HasColumnName("end_date").IsRequired();
            entity.Property(e => e.ProjectName)
                .HasMaxLength(100)
                .HasColumnName("name");
            entity.Property(e => e.ProjectCategoriesId)
                .HasColumnType("int(11)")
                .HasColumnName("project_categories_id");
            entity.Property(e => e.StartDate).IsRequired()
                .HasColumnName("start_date");
            entity.Property(e => e.UsersManagerId)
                .HasMaxLength(255)
                .HasColumnName("users_manager_id");
            entity.Property(e => e.StatusId)
                .HasColumnType("int(11)")
                .HasColumnName("status_id");
            entity.Property(e => e.PriorityId)
                .HasColumnType("int(11)")
                .HasColumnName("priority_id");

            entity.HasOne(d => d.ProjectCategory).WithMany(p => p.Projects)
                .HasForeignKey(d => d.ProjectCategoriesId).IsRequired()
                .HasConstraintName("projects_ibfk_1");

            entity.HasOne(d => d.UsersManager).WithMany(p => p.Projects)
                .HasForeignKey(d => d.UsersManagerId).IsRequired()
                .HasConstraintName("projects_ibfk_2");
        });

        modelBuilder.Entity<ProjectCategory>(entity =>
        {
            entity.HasKey(e => e.CategoryId).HasName("PRIMARY");

            entity.Property(e => e.CategoryName)
                .HasMaxLength(100);
        });

        modelBuilder.Entity<Models.Task>(entity =>
        {
            entity.HasKey(e => e.TaskId).HasName("PRIMARY");

            entity.ToTable("tasks");

            entity.HasIndex(e => e.ProjectId, "projects_id");

            entity.HasIndex(e => e.AssignedUserId, "users_id");

            entity.HasIndex(e => e.StatusId, "status_id");

            entity.HasIndex(e => e.PriorityId, "priority_id");

            entity.Property(e => e.TaskId)
                .HasColumnType("int(11)")
                .HasColumnName("id");
            entity.Property(e => e.Description)
                .HasMaxLength(255)
                .HasColumnName("description");
            entity.Property(e => e.DueDate).HasColumnName("due_date");
            entity.Property(e => e.PriorityId)
                .HasColumnType("int(11)")
                .HasColumnName("priority_id");
            entity.Property(e => e.StatusId)
                .HasColumnType("int(11)")
                .HasColumnName("status_id");
            entity.Property(e => e.ProjectId)
                .HasColumnType("int(11)")
                .HasColumnName("projects_id");

            entity.Property(e => e.Title)
                .HasMaxLength(100)
                .HasColumnName("title");
            entity.Property(e => e.AssignedUserId)
                .HasMaxLength(255)
                .HasColumnName("users_id");

            entity.HasOne(d => d.Project).WithMany(p => p.Tasks)
                .HasForeignKey(d => d.ProjectId)
                .HasConstraintName("tasks_ibfk_1");
        });

        modelBuilder.Entity<TaskAttachment>(entity =>
        {
            entity.HasKey(e => e.AttachmentId).HasName("PRIMARY");

            entity.ToTable("task_attachments");

            entity.HasIndex(e => e.TaskId, "tasks_id");

            entity.Property(e => e.AttachmentId)
                .HasColumnType("int(11)")
                .HasColumnName("id");
            entity.Property(e => e.FileData)
                .HasColumnType("blob")
                .HasColumnName("file_data");
            entity.Property(e => e.FileName)
                .HasMaxLength(255)
                .HasColumnName("file_name");
            entity.Property(e => e.TaskId)
                .HasColumnType("int(11)")
                .HasColumnName("tasks_id");

            entity.HasOne(d => d.Task).WithMany(p => p.TaskAttachments)
                .HasForeignKey(d => d.TaskId)
                .HasConstraintName("task_attachments_ibfk_1");
        });

        modelBuilder.Entity<Status>(entity =>
        {
            entity.HasKey(e => e.StatusId).HasName("PRIMARY");

            entity.ToTable("statuses");

            entity.HasIndex(e => e.StatusId, "status_id");

            entity.Property(e => e.StatusId)
                .HasColumnType("int(11)")
                .HasColumnName("id");
            entity.Property(e => e.StatusName)
                .HasMaxLength(50)
                .HasColumnName("name");


        });

        modelBuilder.Entity<Priority>(entity =>
        {
            entity.HasKey(e => e.PriorityId).HasName("PRIMARY");

            entity.HasIndex(e => e.PriorityDef).IsUnique();
            entity.Property(e => e.PriorityDef)
                .HasMaxLength(50)
                .IsRequired();
        });
    }
}