using Microsoft.EntityFrameworkCore;
using PmsApi.Models;

namespace PmsApi.DataContexts;

public partial class PmsapiContext : DbContext
{
    public PmsapiContext()
    {
    }

    public PmsapiContext(DbContextOptions<PmsapiContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Project> Projects { get; set; }

    public virtual DbSet<ProjectCategory> ProjectCategories { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<Models.Task> Tasks { get; set; }

    public virtual DbSet<TaskAttachment> TaskAttachments { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<Status> Statuses { get; set; }

    public virtual DbSet<Priority> Priorities { get; set; }

    // protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)

    //     => optionsBuilder.UseMySql(connectionString, Microsoft.EntityFrameworkCore.ServerVersion.Parse("11.4.2-mariadb"));

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder
            .UseCollation("latin1_swedish_ci")
            .HasCharSet("latin1");
        try
        {
            modelBuilder.Entity<Project>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("PRIMARY");

                entity.ToTable("projects");

                entity.HasIndex(e => e.ProjectCategoriesId, "project_categories_id");

                entity.HasIndex(e => e.UsersManagerId, "users_manager_id");

                entity.HasIndex(e => e.StatusId, "status_id");

                entity.HasIndex(e => e.PriorityId, "priority_id");
                entity.HasIndex(e => e.Name).IsUnique();

                entity.Property(e => e.Id)
                    .HasColumnType("int(11)")
                    .HasColumnName("id");
                entity.Property(e => e.Description)
                    .HasMaxLength(255)
                    .HasColumnName("description");
                entity.Property(e => e.EndDate)
                    .HasColumnName("end_date").IsRequired();
                entity.Property(e => e.Name)
                    .HasMaxLength(100)
                    .HasColumnName("name");
                entity.Property(e => e.ProjectCategoriesId)
                    .HasColumnType("int(11)")
                    .HasColumnName("project_categories_id");
                entity.Property(e => e.StartDate).IsRequired()
                    .HasColumnName("start_date");
                entity.Property(e => e.UsersManagerId)
                    .HasColumnType("int(11)")
                    .HasColumnName("users_manager_id");
                entity.Property(e => e.StatusId)
                    .HasColumnType("int(11)")
                    .HasColumnName("status_id");
                entity.Property(e => e.PriorityId)
                    .HasColumnType("int(11)")
                    .HasColumnName("priority_id");

                entity.HasOne(d => d.ProjectCategories).WithMany(p => p.Projects)
                    .HasForeignKey(d => d.ProjectCategoriesId).IsRequired()
                    .HasConstraintName("projects_ibfk_1");

                entity.HasOne(d => d.UsersManager).WithMany(p => p.Projects)
                    .HasForeignKey(d => d.UsersManagerId).IsRequired()
                    .HasConstraintName("projects_ibfk_2");
            });

            modelBuilder.Entity<ProjectCategory>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("PRIMARY");

                entity.ToTable("project_categories");

                entity.Property(e => e.Id)
                    .HasColumnType("int(11)")
                    .HasColumnName("id");
                entity.Property(e => e.Name)
                    .HasMaxLength(100)
                    .HasColumnName("name");
            });

            modelBuilder.Entity<Role>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("PRIMARY");

                entity.ToTable("roles");

                entity.Property(e => e.Id)
                    .HasColumnType("int(11)")
                    .HasColumnName("id");
                entity.Property(e => e.Name)
                    .HasMaxLength(50)
                    .HasColumnName("name");
            });

            modelBuilder.Entity<Models.Task>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("PRIMARY");

                entity.ToTable("tasks");

                entity.HasIndex(e => e.ProjectsId, "projects_id");

                entity.HasIndex(e => e.UsersId, "users_id");

                entity.HasIndex(e => e.StatusId, "status_id");

                entity.HasIndex(e => e.PriorityId, "priority_id");

                entity.Property(e => e.Id)
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
                entity.Property(e => e.ProjectsId)
                    .HasColumnType("int(11)")
                    .HasColumnName("projects_id");

                entity.Property(e => e.Title)
                    .HasMaxLength(100)
                    .HasColumnName("title");
                entity.Property(e => e.UsersId)
                    .HasColumnType("int(11)")
                    .HasColumnName("users_id");

                entity.HasOne(d => d.Projects).WithMany(p => p.Tasks)
                    .HasForeignKey(d => d.ProjectsId)
                    .HasConstraintName("tasks_ibfk_1");

                entity.HasOne(d => d.Users).WithMany(p => p.Tasks)
                    .HasForeignKey(d => d.UsersId)
                    .HasConstraintName("tasks_ibfk_2");

                // entity.HasOne(d => d.Status).WithMany(p => p.Tasks)
                // .HasForeignKey(d => d.StatusId)
                // .HasConstraintName("tasks_ibfk_3");
                // entity.HasOne(d => d.Priority).WithMany(p => p.Tasks)
                // .HasForeignKey(d => d.PriorityId)
                // .HasConstraintName("tasks_ibfk_4");



            });

            modelBuilder.Entity<TaskAttachment>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("PRIMARY");

                entity.ToTable("task_attachments");

                entity.HasIndex(e => e.TasksId, "tasks_id");

                entity.Property(e => e.Id)
                    .HasColumnType("int(11)")
                    .HasColumnName("id");
                entity.Property(e => e.FileData)
                    .HasColumnType("blob")
                    .HasColumnName("file_data");
                entity.Property(e => e.FileName)
                    .HasMaxLength(255)
                    .HasColumnName("file_name");
                entity.Property(e => e.TasksId)
                    .HasColumnType("int(11)")
                    .HasColumnName("tasks_id");

                entity.HasOne(d => d.Tasks).WithMany(p => p.TaskAttachments)
                    .HasForeignKey(d => d.TasksId)
                    .HasConstraintName("task_attachments_ibfk_1");
            });

            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(e => e.UserId).HasName("PRIMARY");

                entity.ToTable("users");

                entity.HasIndex(e => e.RoleId, "role_id");

                entity.Property(e => e.UserId)
                    .HasColumnType("int(11)")
                    .HasColumnName("id");
                entity.Property(e => e.Email)
                    .HasMaxLength(100)
                    .HasColumnName("email");
                entity.Property(e => e.FirstName)
                    .HasMaxLength(50)
                    .HasColumnName("first_name");
                entity.Property(e => e.LastName)
                    .HasMaxLength(50)
                    .HasColumnName("last_name");
                entity.Property(e => e.Password)
                    .HasMaxLength(255)
                    .HasColumnName("password");
                entity.Property(e => e.RoleId)
                    .HasColumnType("int(11)")
                    .HasColumnName("role_id");
                entity.Property(e => e.Username)
                    .HasMaxLength(50)
                    .HasColumnName("username");

                entity.HasOne(d => d.Role).WithMany(p => p.Users)
                    .HasForeignKey(d => d.RoleId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("users_ibfk_1");
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

                entity.ToTable("priorities");

                entity.HasIndex(e => e.PriorityId, "priority_id");

                entity.Property(e => e.PriorityId)
                    .HasColumnType("int(11)")
                    .HasColumnName("id");
                entity.Property(e => e.PriorityDef)
                    .HasMaxLength(50)
                    .HasColumnName("name");

            });

            OnModelCreatingPartial(modelBuilder);
        }
        catch (NullReferenceException ex)
        {
            Console.WriteLine(ex.Message);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
